module LoginFlow 

open System.IO
open System.Threading.Tasks
open FSharp.Data
open FSharp.Control.Tasks.V2.ContextInsensitive

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Saturn
open Config

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open Shared.ViewModels
open Shared.Auth
open Shared.Utils
open Shared.Result
open Shared.WsBridge

open ServerUtils
open ServerUtils.TaskResult

open Customer.Wallet
open System
open Microsoft.AspNetCore.Http
open Dapper

open TypeShape.Tools

open Seed 
open System.Security.Cryptography


let issueAuthToken email =
    let userRigths = { AuthJwt.UserRights.UserName = email }
    userRigths |> AuthJwt.encode |> AuthToken

let saveAuthToken connectionString (AuthToken authToken) customerId =
    let auth: AuthTokens.AuthToken = {  AuthToken   = authToken
                                        CustomerId  = customerId
                                        Issued      = DateTime.Now
                                        Expires     = DateTime.Now.AddMonths 1 }
    AuthTokens.Database.insert connectionString auth  

let internal isTokenValid (AuthToken authToken) =
    authToken |> AuthJwt.checkValid |> Option.isSome

let internal checkAuthTokenValid config (AuthToken authToken) = 
    task {
        let! ret = taskResult {
            let! _              = if isTokenValid (AuthToken authToken) then Ok () else Error TokenInvalid
            let! authTokenOpt   = (fun _ -> TokenInvalid), AuthTokens.Database.getById config.connectionString authToken
            let! authToken      = TokenInvalid, authTokenOpt
            let! _              = if authToken.Expires > DateTime.Now then Ok () else Error UserTokenExpired 
            return ()
        }

        return ret |> Result.mapError (fun e -> printfn "AuthToken '%s' validation error '%A'" authToken e; AuthError e) 
    }

let private login config (loginInfo: LoginInfo) : Task<LoginResult> = task {
    let internalError = LoginInternalError >> LoginServerError

    let! ret = taskResult {
        let! customerOpt = internalError, Customers.Database.getByEmail config.connectionString loginInfo.Email
        let! customer = EmailNotFoundOrPasswordIncorrect, customerOpt 

        let authToken = issueAuthToken customer.Email  
        let! code = internalError, saveAuthToken config.connectionString authToken customer.Id 
        let! _    = if code = 0 then Ok () else (new Exception("Auth Token issuing failed") ) |> internalError |> Error

        return authToken 
    }
    do! Async.Sleep 500 |> Async.StartAsTask // Basic brute force protection
    return ret |> Ok 
}

let private register config (loginInfo: LoginInfo) : Task<RegisteringResult> = task {  // TODO: Change this!!!
    let internalError = LoginInternalError >> RegisteringServerError

    let customer = createCustomer loginInfo.Email loginInfo.Password ""

    let! ret = taskResult {
        let! code = internalError, Customers.Database.insert config.connectionString customer
        let! _    = if code = 0 then Ok () else (new Exception("Error while creating customer record") ) |> internalError |> Error

        let authToken = issueAuthToken customer.Email  
        let! code = internalError, saveAuthToken config.connectionString authToken customer.Id 
        let! _    = if code = 0 then Ok () else (new Exception(sprintf "Error when saving authToken, Code: '%d'" code) ) |> internalError |> Error

        return authToken
    }

    do! Async.Sleep 500 |> Async.StartAsTask // Basic brute force protection
    return ret |> Ok
}

let issueResetToken email =
    let token = email + "||" + (AuthJwt.createPassPhrase() |> Text.Encoding.UTF8.GetString)
    token |> AuthJwt.encodeString |> PwdResetToken

let saveResetToken connectionString (PwdResetToken pwdResetToken) customerId =
    let auth: PwdResetTokenInfos.PwdResetTokenInfo = {  PwdResetToken   = pwdResetToken
                                                        CustomerId      = customerId
                                                        Issued          = DateTime.Now
                                                        Expires         = DateTime.Now.AddMinutes 20. }
    PwdResetTokenInfos.Database.insert connectionString auth  

let internal isResetTokenValid (PwdResetToken pwdResetToken) =
    pwdResetToken |> AuthJwt.checkValid |> Option.isSome

let private forgotPassword config (forgotPasswordInfo: ForgotPasswordInfo): Task<ForgotPasswordResult> = task { 
    let internalError = LoginInternalError >> ForgotPasswordServerError
    let notFoundError = sprintf "Customer Email not found: '%s'" forgotPasswordInfo.Email |> EmailNotRegistered

    let! ret = taskResult {
        let! customerOpt    = internalError, Customers.Database.getByEmail config.connectionString forgotPasswordInfo.Email
        let! customer       = notFoundError, customerOpt  

        let resetToken      = issueResetToken forgotPasswordInfo.Email
        let! code           = internalError, saveResetToken config.connectionString resetToken customer.Id
        let! _              = if code = 0 then Ok () else (new Exception("Reset Token issuing failed") ) |> internalError |> Error

        let! mailerResult   = taskResult {  do! internalError, config.resetPasswordEmailer forgotPasswordInfo resetToken
                                            return "Reset email sent" }
        return mailerResult
    }

    do! Async.Sleep 500 |> Async.StartAsTask // Basic brute force protection
    return ret |> Ok
}

let private resetPassword config (resetPasswordInfo: ResetPasswordInfo): Task<PasswordResetResult> = task {
    let notFoundError = ResetTokenNotRecognized resetPasswordInfo.PwdResetToken 
    let tokenError    = ResetTokenExpired resetPasswordInfo.PwdResetToken 
    let internalError = LoginInternalError >> PasswordResetServerError

    let! ret = taskResult {
        let! _                  = if isResetTokenValid resetPasswordInfo.PwdResetToken then Ok () else Error tokenError
        let! pwdResetTokenOpt   = internalError, PwdResetTokenInfos.Database.getByPwdResetToken config.connectionString resetPasswordInfo.PwdResetToken
        let! pwdResetToken      = notFoundError, pwdResetTokenOpt
        let! _                  = if pwdResetToken.Expires > DateTime.Now then Ok () else Error tokenError 
        let! customerOpt        = internalError, Customers.Database.getById config.connectionString pwdResetToken.CustomerId
        let! customer           = notFoundError, customerOpt  
        
        let authToken = issueAuthToken customer.Email  
        let! code = internalError, saveAuthToken config.connectionString authToken customer.Id 
        let! _    = if code = 0 then Ok () else (new Exception("Auth Token issuing failed") ) |> internalError |> Error

        let! code = internalError, PwdResetTokenInfos.Database.update config.connectionString { pwdResetToken with Expires = DateTime.Now }
        let! _    = if code = 0 then Ok () else (new Exception("Reset Token update failed") ) |> internalError |> Error

        return authToken
    }

    do! Async.Sleep 500 |> Async.StartAsTask // Basic brute force protection     
    return ret |> Ok 
}

let loginProtocol config =
    {   login               = login                 config    >> Async.AwaitTask
        register            = register              config    >> Async.AwaitTask
        forgotPassword      = forgotPassword        config    >> Async.AwaitTask
        resetPassword       = resetPassword         config    >> Async.AwaitTask
    }            

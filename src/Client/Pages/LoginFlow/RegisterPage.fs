module Client.RegisterPage

open System
open System.Text.RegularExpressions

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Fable.PowerPack
open Elmish
open Elmish.React

open Shared
open Shared.Auth
open ViewModels

open Client.LoginCommon
open Client.Page

type Msg = 
    // | Login
    | ChangeEmail           of string
    | ChangePassword        of string
    | ChangeConfPassword    of string
    // | LoginSuccess      of authToken: AuthToken
    | RegistrationFailed    of error:string list
    | UpdateValidationErrors 
    | RegisterClicked

type ExternalMsg =
    | NoOp
    | RegisterUser of LoginInfo

type Model = {
    InputEmail                  : string
    InputPassword               : string
    InputPasswordConf           : string
    EmailValidationErrors       : string list
    PasswordValidationErrors    : string list
    PasswordConfValidationErrors: string list
    EmailStartedTyping          : bool
    PasswordStartedTyping       : bool
    PasswordConfStartedTyping   : bool
    TryingToRegister            : bool
    RegisteringErrors           : string list
}

let init userName = 
    {   InputEmail                   = userName
        InputPassword                = ""
        InputPasswordConf            = ""
        EmailValidationErrors     = [ ]
        PasswordValidationErrors     = [ ]
        PasswordConfValidationErrors = [ ]
        EmailStartedTyping           = false
        PasswordStartedTyping        = false
        PasswordConfStartedTyping    = false
        TryingToRegister             = false
        RegisteringErrors            = [ ] }, Cmd.ofMsg UpdateValidationErrors

let update (msg: Msg) model : Model * Cmd<Msg> * ExternalMsg = 
    match msg with
    | ChangeEmail username -> 
        { model with InputEmail = username; EmailStartedTyping = true; RegisteringErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangePassword password ->
        { model with InputPassword = password; PasswordStartedTyping = true; RegisteringErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangeConfPassword password ->
        { model with InputPasswordConf = password; PasswordConfStartedTyping = true; RegisteringErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    // | LoginSuccess token ->
    //     { model with State =    LoggedIn { Token = token; UserName = model.InputUserName }
    //                             InputPassword = "" 
    //                             HasTriedToLogin = false }, Cmd.none, NoOp
    | RegistrationFailed errors -> 
        { model with RegisteringErrors = errors; TryingToRegister = false }, Cmd.none, NoOp
    | UpdateValidationErrors -> 
        { model with    EmailValidationErrors = InputValidators.emailValidation model.InputEmail
                        PasswordValidationErrors = InputValidators.passwordConfValidation model.InputPasswordConf model.InputPassword
                        PasswordConfValidationErrors = InputValidators.passwordConfValidation model.InputPasswordConf model.InputPassword }, Cmd.none, NoOp
    | RegisterClicked ->
        { model with TryingToRegister = true }, Cmd.none, RegisterUser { UserName = model.InputEmail; Password = model.InputPassword } // TODO: hash password


let view model (dispatch: Msg -> unit) =
    let buttonActive =  if not model.RegisteringErrors.IsEmpty 
                            && hasErrors model.EmailStartedTyping model.EmailValidationErrors |> not
                            && hasErrors model.PasswordStartedTyping model.PasswordValidationErrors |> not
                            && (model.EmailStartedTyping || model.PasswordStartedTyping || model.PasswordConfStartedTyping)
                        then "btn-disabled" else "btn-info"        
    div [ Class "login"
            // HTMLAttr.Custom ("style", "background: white; padding: 10% 0px; height: 100vh") 
            ]
            [ div [ Class "middle-box text-center loginscreen  animated fadeInDown" ]
                [     div [ ]
                        [ img [ Alt "image"
                                Class "h55"
                                Src "../lib/img/avalanchain.png" ] ]
                    //   br [ ]
                      h3 [ ]
                        [ str "Register to Token Cabinet" ]
                      p [ ]
                        [ str "Create account to see it in action." ]
                      form [ Class "m-t"
                             Role "form"
                             Action "#" ]
                        [ 
                          div [ Class ("form-group " + hasErrorsClass model.EmailStartedTyping model.EmailValidationErrors) ]
                            [   input [ Id "email"
                                        Type "email" 
                                        ClassName "form-control"
                                        Placeholder "Email address" 
                                        DefaultValue model.InputEmail
                                        OnChange (fun ev -> dispatch (ChangeEmail !!ev.target?value))
                                        AutoFocus true ]
                                hasErrorsSpan model.EmailStartedTyping model.EmailValidationErrors
                            ]
                          div [ Class ("form-group " + hasErrorsClass model.PasswordStartedTyping model.PasswordValidationErrors) ]
                            [   input [ Type "password" 
                                        ClassName "form-control" 
                                        Placeholder "Password"  
                                        DefaultValue model.InputPassword
                                        OnChange (fun ev -> dispatch (ChangePassword !!ev.target?value)) ]
                                hasErrorsSpan model.PasswordStartedTyping model.PasswordValidationErrors 
                            ]
                          div [ Class ("form-group " + hasErrorsClass model.PasswordConfStartedTyping model.PasswordConfValidationErrors) ]
                            [   input [ Type "password" 
                                        ClassName "form-control" 
                                        Placeholder "Confirm Password"  
                                        DefaultValue model.InputPasswordConf
                                        OnChange (fun ev -> dispatch (ChangeConfPassword !!ev.target?value)) ]
                                hasErrorsSpan model.PasswordConfStartedTyping model.PasswordConfValidationErrors 
                            ]
                          a [ 
                              Type "submit"
                              Class "btn btn-info block full-width m-b"
                              OnClick (fun _ -> dispatch RegisterClicked)
                              onEnter RegisterClicked dispatch 
                              ]
                            [ str "Register" ] 
                          p [ Class "text-muted text-center" ]
                            [ small [ ]
                                [ str "Already have an account?" ] ]
                          a [ Class "btn btn-sm btn-white btn-block"
                              Href (LoginFlowPage.Login |> MenuPage.LoginFlow |> toHash) 
                              OnClick goToUrl ]
                            [ str "Login" ]]
                      p [ Class "m-t project-title" ]
                        [ small [ ]
                            [ str "powered by "
                              a [ 
                                //   HTMLAttr.Custom ("style", "font-size: 12px;")
                                  Href "http://avalanchain.com" ]
                                [ str "Avalanchain" ]
                              str " © 2018" ] ] ] ]

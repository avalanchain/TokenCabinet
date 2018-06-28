module Client.ForgotPasswordPage

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


type Msg = 
    | Login
    | ChangeUserName    of string
    | ChangePassword    of string
    | LoginSuccess      of authToken: AuthToken
    | LoginFailed       of error:string
    | UpdateValidationErrors 
    | LogInClicked

type ExternalMsg =
    | NoOp
    | LoginUser of LoginInfo


type Model = {
    State : LoginState
    InputUserName: string
    UsernameValidationErrors: string list
    PasswordValidationErrors: string list
    InputPassword: string
    HasTriedToLogin: bool
    LoginError: string option
}

let init (authModel: AuthModel option) = 
    let state, userName = match authModel with
                            | None              -> LoggedOut, ""
                            | Some authModel    -> LoggedIn authModel, authModel.UserName
    {   State         = state
        InputUserName = userName
        InputPassword = ""
        UsernameValidationErrors =  [ ]
        PasswordValidationErrors =  [ ]
        HasTriedToLogin = false
        LoginError      = None }, Cmd.ofMsg UpdateValidationErrors

let validateInput (model: Model) =  
    let usernameRules = 
        [   String.IsNullOrWhiteSpace(model.InputUserName), "Field 'Username' cannot be empty"
            model.InputUserName.Trim().Length < 5, "Field 'Username' must at least have 5 characters" ]
    let passwordRules = 
        [   String.IsNullOrWhiteSpace(model.InputPassword), "Field 'Password' cannot be empty"
            model.InputPassword.Trim().Length < 8, "Field 'Password' must at least have 8 characters"
            Regex("""(?=.*[a-z])""").IsMatch(model.InputPassword), "Field 'Password' must have at least 1 lowercase character"
            Regex("""(?=.*[A-Z])""").IsMatch(model.InputPassword), "Field 'Password' must have at least 1 uppercase character"
            Regex("""(?=.*[\d])""").IsMatch(model.InputPassword), "Field 'Password' must have at least 1 digit character"
            Regex("""(?=.*[\W])""").IsMatch(model.InputPassword), "Field 'Password' must have at least 1 special character"
            ]

    let usernameValidationErrors = usernameRules |> List.filter fst |> List.map snd
    let passwordValidationErrors = passwordRules |> List.filter fst |> List.map snd
    usernameValidationErrors, passwordValidationErrors

let update (msg: Msg) model : Model * Cmd<Msg> * ExternalMsg = 
    match msg with
    | Login -> 
        { model with InputPassword = ""; LoginError = None }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangeUserName username -> 
        { model with InputUserName = username; InputPassword = ""; LoginError = None }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangePassword password ->
        { model with InputPassword = password; LoginError = None }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | LoginSuccess token ->
        { model with State =    LoggedIn { Token = token; UserName = model.InputUserName }
                                InputPassword = "" 
                                HasTriedToLogin = false }, Cmd.none, NoOp
    | LoginFailed error -> 
        { model with LoginError = Some error; HasTriedToLogin = false }, Cmd.none, NoOp
    | UpdateValidationErrors -> 
        let usernameValidationErrors, passwordValidationErrors = validateInput model
        { model with    UsernameValidationErrors = usernameValidationErrors
                        PasswordValidationErrors = passwordValidationErrors }, Cmd.none, NoOp
    | LogInClicked ->
        { model with HasTriedToLogin = true }, Cmd.none, LoginUser { UserName = model.InputUserName; Password = model.InputPassword } // TODO: hash password



let view model (dispatch: Msg -> unit) =
    div [ Class "login"
            // HTMLAttr.Custom ("style", "background: white; padding: 10% 0px; height: 100vh") 
            ]
            [ div [ Class "middle-box text-center animated fadeInDown" ]
                [ div [ ]
                        [ 
                          div [ Class "col-md-12" ]
                                [ div [ Class "ibox-content" ]
                                    [ div [ ]
                                          [ img [ Alt "image"
                                                  Class "h55"
                                                  Src "../lib/img/avalanchain.png" ] ]
                                      h2 [ Class "font-bold" ]
                                        [ str "Forgot password" ]
                                      p [ ]
                                        [ str "Enter your email address and your password will be reset and emailed to you." ]
                                      div [ Class "row" ]
                                        [ div [ Class "col-lg-12" ]
                                            [ form [ Class "m-t"
                                                     Role "form"
                                                     Action "index.html" ]
                                                [ div [ Class "form-group" ]
                                                    [ input [ Type "email"
                                                              Class "form-control"
                                                              Placeholder "Email address" ] ]
                                                  button [ Type "submit"
                                                           Class "btn btn-info block full-width m-b" ]
                                                    [ str "Send new password" ]
                                                  p [ Class "text-muted text-center" ]
                                                    [ small [ ]
                                                        [ str "Go back to Login" ] ]
                                                  a [ Class "btn btn-sm btn-white btn-block"
                                                      Href "login.html" ]
                                                    [ str "Login" ] ] ] ] ] ]  
                          p [ Class "m-t project-title" ]
                            [ small [ ]
                                [ str "powered by "
                                  a [ 
                                    //   HTMLAttr.Custom ("style", "font-size: 12px;")
                                      Href "http://avalanchain.com" ]
                                    [ str "Avalanchain" ]
                                  str " © 2018" ] ] ] ] ]

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
    | Login
    | ChangeUserName    of string
    | ChangePassword    of string
    | LoginSuccess      of authToken: AuthToken
    | LoginFailed       of error:string
    | UpdateValidationErrors 
    | LogInClicked

type ExternalMsg =
    | NoOp
    | RegisterUser of LoginInfo

type Model = {
    InputUserName: string
    InputPassword: string
    InputPasswordConf: string
    UsernameValidationErrors: string list
    PasswordValidationErrors: string list
    PasswordConfValidationErrors: string list
    HasTriedToLogin: bool
    RegisteringError: string option
}

let init userName = 
    {   InputUserName     = userName
        InputPassword     = ""
        InputPasswordConf = ""
        UsernameValidationErrors     =  [ ]
        PasswordValidationErrors     =  [ ]
        PasswordConfValidationErrors =  [ ]
        HasTriedToLogin     = false
        RegisteringError    = None }, Cmd.ofMsg UpdateValidationErrors

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
        { model with InputPassword = ""; RegisteringError = None }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangeUserName username -> 
        { model with InputUserName = username; InputPassword = ""; RegisteringError = None }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangePassword password ->
        { model with InputPassword = password; RegisteringError = None }, Cmd.ofMsg UpdateValidationErrors, NoOp
    // | LoginSuccess token ->
    //     { model with State =    LoggedIn { Token = token; UserName = model.InputUserName }
    //                             InputPassword = "" 
    //                             HasTriedToLogin = false }, Cmd.none, NoOp
    | LoginFailed error -> 
        { model with RegisteringError = Some error; HasTriedToLogin = false }, Cmd.none, NoOp
    | UpdateValidationErrors -> 
        let usernameValidationErrors, passwordValidationErrors = validateInput model
        { model with    UsernameValidationErrors = usernameValidationErrors
                        PasswordValidationErrors = passwordValidationErrors }, Cmd.none, NoOp
    | LogInClicked ->
        { model with HasTriedToLogin = true }, Cmd.none, RegisterUser { UserName = model.InputUserName; Password = model.InputPassword } // TODO: hash password




let view model (dispatch: Msg -> unit) =
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
                          div [ Class "form-group" ]
                            [ input [ Id "Email"
                                      Type "email" 
                                      ClassName "form-control"
                                      Placeholder "Email" 
                                      // DefaultValue model.InputUserName
                                      // OnChange (fun ev -> dispatch (ChangeUserName !!ev.target?value))
                                      AutoFocus true ] ]
                          div [ Class "form-group" ]
                            [ input [ Type "password" 
                                      ClassName "form-control" 
                                      Placeholder "Password"  
                                      // DefaultValue model.InputUserName
                                      // OnChange (fun ev -> dispatch (ChangePassword !!ev.target?value))
                                      // onEnter LogInClicked dispatch 
                                      ] ]
                          div [ Class "form-group" ]
                            [ input [ Type "password" 
                                      ClassName "form-control" 
                                      Placeholder "Confirm Password"  
                                      // DefaultValue model.InputUserName
                                      // OnChange (fun ev -> dispatch (ChangePassword !!ev.target?value))
                                      // onEnter LogInClicked dispatch 
                                      ] ]
                          a [ 
                              Type "submit"
                              Class "btn btn-info block full-width m-b"
                              // OnClick (fun _ -> dispatch LogInClicked) 
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

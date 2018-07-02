module Client.LoginPage

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

open Client.LoginCommon
open Client.Page
open Shared
open Shared.InputValidators
open Fable.Import.React

type Msg = 
    // | Login
    | ChangeEmail           of string
    | ChangePassword        of string
    // | LoginSuccess           of authToken: AuthToken
    | LoginFailed           of errors:string list
    | UpdateValidationErrors 
    | LogInClicked

type ExternalMsg =
    | NoOp
    | LoginUser of LoginInfo


type Model = {
    InputEmail              : string
    InputPassword           : string
    EmailValidationErrors   : string list
    PasswordValidationErrors: string list
    EmailStartedTyping      : bool
    PasswordStartedTyping   : bool
    TryingToLogin           : bool
    LoginErrors             : string list
}

let init email = 
    {   InputEmail               = email
        InputPassword            = ""
        EmailValidationErrors    =  [ ]
        PasswordValidationErrors =  [ ]
        EmailStartedTyping       = false
        PasswordStartedTyping    = false
        TryingToLogin            = false
        LoginErrors              = [] }, Cmd.ofMsg UpdateValidationErrors

let update (msg: Msg) model : Model * Cmd<Msg> * ExternalMsg = 
    match msg with
    // | Login -> 
    //     { model with InputPassword = ""; LoginError = None }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangeEmail username -> 
        { model with InputEmail = username; EmailStartedTyping = true; LoginErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangePassword password ->
        { model with InputPassword = password; PasswordStartedTyping = true; LoginErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    // | LoginSuccess token ->
    //     { model with State =    LoggedIn { Token = token; UserName = model.InputUserName }
    //                             InputPassword = "" 
    //                             HasTriedToLogin = false }, Cmd.none, NoOp
    | LoginFailed error -> 
        { model with LoginErrors = []; TryingToLogin = false }, Cmd.none, NoOp
    | UpdateValidationErrors -> 
        { model with    EmailValidationErrors = InputValidators.emailValidation model.InputEmail
                        PasswordValidationErrors = InputValidators.passwordValidation model.InputPassword }, Cmd.none, NoOp
    | LogInClicked ->
        { model with TryingToLogin = true }, Cmd.none, LoginUser { UserName = model.InputEmail; Password = model.InputPassword } // TODO: hash password


    // | SetUserName name ->
    //     { model with Login = { model.Login with UserName = name; Password = "" }}, []
    // | SetPassword pw ->
    //     { model with Login = { model.Login with Password = pw }}, []
    // | ClickLogIn ->
    //     model, authUserCmd model.Login "/api/users/login"
    // | AuthError exn ->
    //     { model with ErrorMsg = string (exn.Message) }, []

let [<Literal>] ENTER_KEY = 13.

[<Emit("null")>]
let emptyElement : ReactElement = jsNative
let hasErrors startedTyping (errors: List<_>) = startedTyping && not errors.IsEmpty 
let hasErrorsClass startedTyping (errors: List<_>) =
    if hasErrors startedTyping errors then "has-error" else ""
let hasErrorsSpan startedTyping (errors: List<_>) =
    if hasErrors startedTyping errors 
    then span [ Class "help-block" ] 
            [ ul [ ]  [ for error in errors -> li [ ] [ str error ] ] ]
    else emptyElement        

let errorMessagesIfAny startedTyping = function
    | [ ] -> emptyElement
    | _ when not startedTyping -> emptyElement 
    | errors -> span [ Class "help-block" ] 
                    [ ul [ ]  [ for error in errors -> li [ ] [ str error ] ] ]

let view (model: Model) (dispatch: Msg -> unit) = 
    // let showErrorClass = if String.IsNullOrEmpty model.ErrorMsg then "hidden" else ""
    let buttonActive =  if not model.LoginErrors.IsEmpty 
                            && not model.EmailValidationErrors.IsEmpty
                            && not model.PasswordValidationErrors.IsEmpty 
                        then "btn-disabled" else "btn-info"

    let onEnter msg dispatch =
        function 
        | (ev:React.KeyboardEvent) when ev.keyCode = ENTER_KEY ->
            ev.preventDefault() 
            dispatch msg
        | _ -> ()
        |> OnKeyDown
        

    div [ Class "login"
        // HTMLAttr.Custom ("style", "background: white; padding: 10% 0px; height: 100vh") 
        ]
        [ div [ Class "middle-box text-center loginscreen  animated fadeInDown" ]
              [   div [ ]
                    [ img [ Alt "image"
                            Class "h55"
                            Src "../lib/img/avalanchain.png" ] ]
                //   br [ ]
                  h3 [ ]
                    [ str "Welcome to avalanchain" ]
                  form [ Class "m-t"
                         Role "form"
                         Action "#" ]
                    [ div [ Class ("form-group " + hasErrorsClass model.EmailStartedTyping model.EmailValidationErrors) ]
                        [   input [ Id "email"
                                    Type "email" 
                                    ClassName "form-control"
                                    Placeholder "Email" 
                                    DefaultValue model.InputEmail
                                    OnChange (fun ev -> dispatch (ChangeEmail !!ev.target?value))
                                    AutoFocus true ]
                            errorMessagesIfAny model.EmailStartedTyping model.EmailValidationErrors

                        //   if not model.EmailValidationErrors.IsEmpty then 
                        //     yield span [ Id "helpBlockUserName"; Class "help-block" ]
                        //             (model.EmailValidationErrors |> List.map str)  
                        //   match model.LoginError with
                        //   | Some e ->  
                        //     yield span [ Id "helpBlockServerValidationResult"; Class "help-block" ]
                        //             [ str e ]
                        //   | None -> ()                                 
                        ]
                      div [ Class ("form-group " + hasErrorsClass model.PasswordStartedTyping model.PasswordValidationErrors) ]
                        [   input [ Type "password" 
                                    ClassName "form-control" 
                                    Placeholder "Password"  
                                    DefaultValue model.InputPassword
                                    OnChange (fun ev -> dispatch (ChangePassword !!ev.target?value))
                                    onEnter LogInClicked dispatch ]
                        //   if not model.EmailValidationErrors.IsEmpty then 
                        //     yield span [ Id "helpBlockPassword"; Class "help-block" ]
                        //             (model.PasswordValidationErrors |> List.map str)
                            errorMessagesIfAny model.PasswordStartedTyping model.PasswordValidationErrors 
                        ]
                      a [ 
                          Type "submit"
                          Class ("btn block full-width m-b " + buttonActive)
                          OnClick (fun _ -> dispatch LogInClicked) ]
                        [ str "Login" ] 
                      a [   Href (LoginFlowPage.ForgotPassword |> MenuPage.LoginFlow |> toHash) 
                            OnClick goToUrl ]
                        [ small [ ]
                            [ str "Forgot password?" ] ]
                      p [ Class "text-muted text-center" ]
                        [ small [ ]
                            [ str "Do not have an account?" ] ]
                      a [   Class "btn btn-sm btn-white btn-block"
                            Href (LoginFlowPage.Register |> MenuPage.LoginFlow |> toHash) 
                            OnClick goToUrl ]
                       [ str "Create an account" ]]
                  p [ Class "m-t project-title" ]
                    [ small [ ]
                        [ str "powered by "
                          a [ 
                            //   HTMLAttr.Custom ("style", "font-size: 12px;")
                              Href "http://avalanchain.com" ]
                            [ str "Avalanchain" ]
                          str " © 2018" ] ] ] ]


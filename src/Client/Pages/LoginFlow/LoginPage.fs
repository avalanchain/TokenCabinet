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
    | ChangeEmail           of string
    | ChangePassword        of string
    | LoginAttemptResult    of Result<AuthToken, LoginError>
    | UpdateValidationErrors 
    | LogInClicked

type ExternalMsg =
    | NoOp
    | LoginUser     of LoginInfo
    | UserLoggedIn  of AuthToken


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
        EmailValidationErrors    = [ ]
        PasswordValidationErrors = [ ]
        EmailStartedTyping       = false
        PasswordStartedTyping    = false
        TryingToLogin            = false
        LoginErrors              = [ ] }, Cmd.ofMsg UpdateValidationErrors

let update (msg: Msg) model : Model * Cmd<Msg> * ExternalMsg = 
    match msg with
    | ChangeEmail username -> 
        { model with InputEmail = username; EmailStartedTyping = true; LoginErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | ChangePassword password ->
        { model with InputPassword = password; PasswordStartedTyping = true; LoginErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    | LoginAttemptResult res -> 
        match res with
        | Ok authToken -> { model with TryingToLogin = false }, Cmd.none, UserLoggedIn authToken
        | Error e -> match e with 
                        | EmailNotFoundOrPasswordIncorrect -> { model with LoginErrors = [ "Email or password incorrect" ]; TryingToLogin = false }, Cmd.none, NoOp
                        | LoginError.LoginServerError e -> { model with LoginErrors = handleLoginFlowServerError e; TryingToLogin = false }, Cmd.none, NoOp
    | UpdateValidationErrors -> 
        { model with    EmailValidationErrors = InputValidators.emailValidation model.InputEmail
                        PasswordValidationErrors = InputValidators.passwordValidation model.InputPassword }, Cmd.none, NoOp
    | LogInClicked ->
        { model with TryingToLogin = true }, Cmd.none, LoginUser { UserName = model.InputEmail; Password = model.InputPassword } // TODO: hash password


let view (model: Model) (dispatch: Msg -> unit) = 
    let buttonActive =  if not model.LoginErrors.IsEmpty 
                            && hasErrors model.EmailStartedTyping model.EmailValidationErrors |> not
                            && hasErrors model.PasswordStartedTyping model.PasswordValidationErrors |> not
                            && (model.EmailStartedTyping || model.PasswordStartedTyping)
                        then "btn-disabled" else "btn-info"        

    div [ Class "login"
        // HTMLAttr.Custom ("style", "background: white; padding: 10% 0px; height: 100vh") 
        ]
        [ div [ Class "middle-box text-center loginscreen  animated fadeInDown" ]
              [   div [ ]
                    [ img [ Alt "image"
                            Class "h90"
                            Src "../lib/img/token_cab_1.png" ] ]
                  form [ Class "m-t"
                         Role "form"
                         Action "#" ]
                    [ div [ Class ("form-group " + hasErrorsClass model.EmailStartedTyping model.EmailValidationErrors) ]
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
                                    OnChange (fun ev -> dispatch (ChangePassword !!ev.target?value))
                                 ]
                            hasErrorsSpan model.PasswordStartedTyping model.PasswordValidationErrors 
                        ]
                      a [ Type "submit"
                          Class ("btn block full-width m-b " + buttonActive)
                          OnClick (fun _ -> dispatch LogInClicked)
                          onEnter LogInClicked dispatch 
                          ]
                        [  (if model.TryingToLogin then i [ ClassName "fa fa-circle-o-notch fa-spin" ] [] 
                            else str "Login") ] 
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


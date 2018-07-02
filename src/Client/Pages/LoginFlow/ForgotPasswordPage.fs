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
open Client.Page


type Msg = 
    | ChangeEmail       of string
    // | LoginSuccess      of authToken: AuthToken
    | UpdateValidationErrors 
    | PasswordResetClicked

type ExternalMsg =
    | NoOp
    | ForgotPassword of ForgotPasswordInfo


type Model = {
    InputEmail              : string
    EmailValidationErrors   : string list
    EmailStartedTyping      : bool
    TryingToReset           : bool
    PasswordResetErrors     : string list
}

let init email = 
    {   InputEmail               = email
        EmailValidationErrors    = [ ]
        EmailStartedTyping       = false
        TryingToReset            = false
        PasswordResetErrors      = [ ] }, Cmd.ofMsg UpdateValidationErrors

let update (msg: Msg) model : Model * Cmd<Msg> * ExternalMsg = 
    match msg with
    | ChangeEmail username -> 
        { model with InputEmail = username; EmailStartedTyping = true; PasswordResetErrors = [] }, Cmd.ofMsg UpdateValidationErrors, NoOp
    // | LoginSuccess token ->
    //     { model with State =    LoggedIn { Token = token; UserName = model.InputUserName }
    //                             InputPassword = "" 
    //                             HasTriedToLogin = false }, Cmd.none, NoOp
    // | LoginFailed error -> 
    //     { model with LoginError = Some error; HasTriedToLogin = false }, Cmd.none, NoOp
    | UpdateValidationErrors -> 
        { model with    EmailValidationErrors = InputValidators.emailValidation model.InputEmail }, Cmd.none, NoOp
    | PasswordResetClicked ->
        { model with TryingToReset = true }, Cmd.none, ForgotPassword { UserName = model.InputEmail } // TODO: hash password



let view model (dispatch: Msg -> unit) =
    let buttonActive =  if not model.PasswordResetErrors.IsEmpty 
                            && hasErrors model.EmailStartedTyping model.EmailValidationErrors |> not
                            && (model.EmailStartedTyping)
                        then "btn-disabled" else "btn-info" 
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
                                                [ div [ Class ("form-group " + hasErrorsClass model.EmailStartedTyping model.EmailValidationErrors) ]
                                                    [ input [ Id "email"
                                                              Type "email"
                                                              Class "form-control"
                                                              Placeholder "Email address"
                                                              DefaultValue model.InputEmail
                                                              OnChange (fun ev -> dispatch (ChangeEmail !!ev.target?value))
                                                              AutoFocus true ]
                                                      hasErrorsSpan model.EmailStartedTyping model.EmailValidationErrors
                                                    ]
                                                  button [  Type "submit"
                                                            Class "btn btn-info block full-width m-b"
                                                            OnClick (fun _ -> dispatch PasswordResetClicked)
                                                            onEnter PasswordResetClicked dispatch ]
                                                    [ str "Reset password" ]
                                                  p [ Class "text-muted text-center" ]
                                                    [ small [ ]
                                                        [ str "Go back to Login" ] ]
                                                  a [ Class "btn btn-sm btn-white btn-block"
                                                      Href (LoginFlowPage.Login |> MenuPage.LoginFlow |> toHash) 
                                                      OnClick goToUrl ]
                                                    [ str "Login" ] ] ] ] ] ]  
                          p [ Class "m-t project-title" ]
                            [ small [ ]
                                [ str "powered by "
                                  a [ 
                                    //   HTMLAttr.Custom ("style", "font-size: 12px;")
                                      Href "http://avalanchain.com" ]
                                    [ str "Avalanchain" ]
                                  str " © 2018" ] ] ] ] ]

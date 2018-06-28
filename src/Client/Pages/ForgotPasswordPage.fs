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
open ViewModels




let view  =
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

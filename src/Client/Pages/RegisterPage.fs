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
open ViewModels




let view  =
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
                              Href "login.html" ]
                            [ str "Login" ]]
                      p [ Class "m-t project-title" ]
                        [ small [ ]
                            [ str "powered by "
                              a [ 
                                //   HTMLAttr.Custom ("style", "font-size: 12px;")
                                  Href "http://avalanchain.com" ]
                                [ str "Avalanchain" ]
                              str " © 2018" ] ] ] ]

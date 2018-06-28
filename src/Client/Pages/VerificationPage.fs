module Client.VerificationPage

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


// let bodyRowSomeNone (model: Model) body =
//     match model.TokenSale with
//     | Some m -> Ibox.btRow "Timeline" ([ body m ])
//     | None   -> Ibox.btRow "Timeline" ([ str "No model loaded" ]) 

let body =
    div [ ]
        [ div [ Class "col-lg-8 b-r" ]
            [ form [ Role "form"
                     Name "caForm"
                     Class "form-horizontal" ]
                [ div [ Class "form-group" ]
                    [ label [ Class "col-sm-2 control-label" ]
                        [ str "First Name" ]
                      div [ Class "col-sm-10" ]
                        [ input [ Type "text"
                                  Class "form-control" ]
                          span [ Class "help-block m-b-none" ]
                            [ str "Chose a name for your Tokens" ] ] ]
                  div [ Class "hr-line-dashed" ]
                    [ ]
                  div [ Class "form-group" ]
                    [ label [ Class "col-sm-2 control-label" ]
                        [ str "Last Name" ]
                      div [ Class "col-sm-10" ]
                        [ input [ Type "text"
                                  Class "form-control" ]
                          span [ Class "help-block m-b-none" ]
                            [ str "Enter a brief description." ] ] ]
                  div [ Class "hr-line-dashed" ]
                    [ ]
                  div [ ]
                    [ button [ Class "btn btn-sm btn-primary pull-right m-t-n-xs"
                               Type "submit" ]
                        [ strong [ ]
                            [ str "Create" ] ] ] ] ]
          div [ Class "col-lg-4" ]
            [ h4 [ ]
                [ str "Assets icon" ]
              p [ ]
                [ str "You can load your asset icon:" ]
              p [ Class "text-center" ]
                [ a [ Href "" ]
                    [ i [ Class "fa fa-upload big-icon" ]
                        [ ] ] ] ]]

let view = Ibox.btRow "Verification" ([ Ibox.emptyRow [body] ])
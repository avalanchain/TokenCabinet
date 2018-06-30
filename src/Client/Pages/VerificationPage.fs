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

let personalData =
    div [ Class "panel-body" ]
        [ div [ Class "col-lg-6 b-r" ]
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
                   ] ]
          div [ Class "col-lg-6" ]
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
                    [ button [ Class "btn btn-sm btn-info pull-right m-t-n-xs"
                               Type "submit" ]
                        [ strong [ ]
                            [ str "Next" ] ] ] ] ]]
let address =
    div [ Class "panel-body" ]
        [ div [ Class "col-lg-12 b-r" ]
            [ form [ Role "form"
                     Name "caForm"
                     Class "form-horizontal" ]
                [ div [ Class "form-group" ]
                    [ label [ Class "col-sm-2 control-label" ]
                        [ str "Country" ]
                      div [ Class "col-sm-10" ]
                        [ input [ Type "text"
                                  Class "form-control" ]
                          span [ Class "help-block m-b-none" ]
                            [ str "Chose a name for your Tokens" ] ] ]
                  div [ Class "hr-line-dashed" ]
                    [ ]
                  div [ Class "form-group" ]
                    [ label [ Class "col-sm-2 control-label" ]
                        [ str "City" ]
                      div [ Class "col-sm-10" ]
                        [ input [ Type "text"
                                  Class "form-control" ]
                          span [ Class "help-block m-b-none" ]
                            [ str "Enter a brief description." ] ] ]
                  div [ Class "hr-line-dashed" ]
                    [ ]
                  div [ ]
                    [ button [ Class "btn btn-sm btn-info pull-right m-t-n-xs"
                               Type "submit" ]
                        [ strong [ ]
                            [ str "Next" ] ] ] ] ]
         ]

let documentation =
    div [ Class "panel-body" ]
        [ 
          div [  ]
            [ h4 [ ]
                [ str "Documentation" ]
              p [ ]
                [ str "You can load your documents" ]
              p [ Class "text-center" ]
                [ a [ Href "" ]
                    [ i [ Class "fa fa-upload big-icon" ]
                        [ ] ] ] ]]
open Helpers
open ReactBootstrap


let tabs = comF tabs (fun o -> 
                           o.defaultActiveKey <- Some (1 :> obj)
                           o.id <- Some "uncontrolled-tab-example") 
                           [
                              comF tab (fun o -> o.eventKey <- Some (1 :> obj) 
                                                 o.title <- Some "Personal data" ) [
                               personalData
                                  ]
                              comF tab (fun o -> o.eventKey <- Some (2 :> obj) 
                                                 o.title <- Some "Registration address" ) [
                              address
                                  ]
                              comF tab (fun o -> o.eventKey <- Some (3 :> obj) 
                                                 o.title <- Some "Documentation" ) [
                              documentation
                                  ]
                            ]

let view = div [ Class "tabs-container"] [tabs]


// let view = Ibox.btRow "Verification" ([ Ibox.emptyRow [body]
//                                         tabs ])
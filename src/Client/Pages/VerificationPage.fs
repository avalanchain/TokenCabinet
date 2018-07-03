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
open Client.FormHelpers

open ReactBootstrap
open Helpers

// let bodyRowSomeNone (model: Model) body =
//     match model.TokenSale with
//     | Some m -> Ibox.btRow "Timeline" ([ body m ])
//     | None   -> Ibox.btRow "Timeline" ([ str "No model loaded" ]) 

let personalData =
    div [ Class "panel-body" ]
        [ div [ Class "col-lg-6 b-r" ]
            [ formHorizontal
                [ 
                  fGroupO (FormElement.Input InputType.Text) (None) "First Name" "Enter a brief First Name"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Input InputType.Text) (None) "Last Name" "Enter a brief Last Name"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Input InputType.Text) (None) "Middle Name" "Enter a brief Middle Name"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Select [ "Sex", ""; "Male", "Male"; "Female", "Female"]) (None) "Sex" "Enter a brief description"
                   ] ]
          div [ Class "col-lg-6" ]
            [ 
              formHorizontal
                [ 
                  fGroupO (FormElement.Input InputType.Date) (None) "Birthday" "Enter a brief Birthday"
                  div [ Class "hr-line-dashed" ] [ ]
                  
                  fGroupO (FormElement.Input InputType.Text) (None) "Number of the document" "Enter a Series and number of the document"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Input InputType.Text) (None) "Country of issue" "Enter a Country of issue of the document"
                  div [ Class "hr-line-dashed" ] [ ]
                  
                  fGroupO (FormElement.Input InputType.Date ) (None) "Registration Date" "Enter a Registration Date"
               
                  div [ ]
                    [ comF button (fun o -> o.bsClass <- Some "btn btn-sm btn-info pull-right m-t-n-xs")
                        [ strong [ ]
                            [ str "Next" ] ] ] ] ]]
let address =
    div [ Class "panel-body" ]
        [ div [ Class "col-lg-12 b-r" ]
            [ formHorizontal
                [ fGroupO (FormElement.Input InputType.Date) (None) "Country" "Enter a Country"
                  div [ Class "hr-line-dashed" ] [ ]
                  
                  fGroupO (FormElement.Input InputType.Text) (None) "City" "Enter a City"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Input InputType.Text) (None) "Address" "Enter a Address"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Input InputType.Text) (None) "Zipcode" "Enter a Zipcode"

                  div [ ]
                    [ comF button (fun o -> o.bsClass <- Some "btn btn-sm btn-info pull-left m-t-n-xs")
                        [ strong [ ]
                            [ str "Prev" ] ] ]
                  
                  div [ ]
                    [ comF button (fun o -> o.bsClass <- Some "btn btn-sm btn-info pull-right m-t-n-xs")
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
                        [ ] ] ] ]
                        
          div [ ]
                    [ comF button (fun o -> o.bsClass <- Some "btn btn-sm btn-info pull-left m-t-n-xs")
                        [ strong [ ]
                            [ str "Prev" ] ] ]
                                
                        ]



let tabs = comF tabs (fun o -> 
                           o.defaultActiveKey <- Some (1 :> obj)
                           o.id <- Some "verification"
                           o.animation <- Some false ) 
                           [
                              comF tab (fun o -> o.eventKey <- Some (1 :> obj) 
                                                 o.title <- Some "Personal data" ) 
                                        [ personalData ]

                              comF tab (fun o -> o.eventKey <- Some (2 :> obj) 
                                                 o.title <- Some "Registration address" ) 
                                        [ address ]

                              comF tab (fun o -> o.eventKey <- Some (3 :> obj) 
                                                 o.title <- Some "Documentation" ) 
                                        [ documentation ]
                            ]

let view = div [ Class "tabs-container"] [tabs]


// let view = Ibox.btRow "Verification" ([ Ibox.emptyRow [body]
//                                         tabs ])
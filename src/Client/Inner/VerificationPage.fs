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

open Cabinet
open CabinetModel
open ReactBootstrap
open Helpers


let helper = div [ Class "border-bottom ibox-content m-b-sm" ] [
                div [ Class "p-xs" ] 
                  [
                    div [ Class "pull-left m-r-md" ]
                        [ i [ Class "fa fa-address-card text-info mid-icon" ] [ ] ]    
                    h2 [ ] [
                       str "AML/KYC Verification" ]
                    span [ ] [
                            str "Please, fill out the form. It will take several minutes. "
                   ]]]

let buttonTab (sideClass:string) (name:string) (key:int) (dispatch) = 
    // comF copyToClipboard (fun o ->  o.text <- address
    //                                 o.onCopy <- (fun (addr, b) -> addr |> AddressCopied |> dispatch) )
    //     [ 
            comF button (fun o -> o.bsClass <- "btn btn-sm btn-info btn-outline btn-w-sm m-t-n-xs " + sideClass  |> Some
                                  o.onClick <- React.MouseEventHandler(fun _ -> key |> TabChanged |> VerificationMsg |> dispatch) |> Some) 
                  [  strong [ ]
                            [ str name ] ]
        //] 
let personalData model dispatch =
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
                    [ 
                       buttonTab "pull-right" "Next" 2 dispatch
                    //    comF button (fun o -> o.bsClass <- Some "btn btn-sm btn-info pull-right m-t-n-xs")
                    //         [ strong [ ]
                    //         [ str "Next" ] ] 
                            
                            ] ] ]]
let address model dispatch =
    div [ Class "panel-body" ]
        [ div [ Class "col-lg-12" ]
            [ formHorizontal
                [ fGroupO (FormElement.Input InputType.Date) (None) "Country" "Enter a Country"
                  div [ Class "hr-line-dashed" ] [ ]
                  
                  fGroupO (FormElement.Input InputType.Text) (None) "City" "Enter a City"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Input InputType.Text) (None) "Address" "Enter a Address"
                  div [ Class "hr-line-dashed" ] [ ]

                  fGroupO (FormElement.Input InputType.Text) (None) "Zipcode" "Enter a Zipcode"

                  div [ ]
                    [ buttonTab "pull-left" "Prev" 1 dispatch ]
                  
                  div [ ]
                    [ buttonTab "pull-right" "Next" 3 dispatch ] ] ]
         ]

let documentation model dispatch =
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
                    [ buttonTab "pull-left" "Prev" 2 dispatch ]
                                
                        ]



let tabs (model:VerificationModel) dispatch = 
                comF tabs (fun o -> 
                           o.activeKey <- Some (model.CurrentTab :> obj)
                           o.id <- Some "verification"
                           o.onSelect <- SelectCallback(fun k -> k.Value :?> int |> TabChanged |> VerificationMsg |> dispatch ) |> Some
                           o.animation <- Some false ) 
                           [
                              comF tab (fun o -> o.eventKey <- Some (1 :> obj) 
                                                 o.title <- Some "Personal data" ) 
                                        [ personalData model dispatch]

                              comF tab (fun o -> o.eventKey <- Some (2 :> obj) 
                                                 o.title <- Some "Registration address" ) 
                                        [ address model dispatch ]

                              comF tab (fun o -> o.eventKey <- Some (3 :> obj) 
                                                 o.title <- Some "Documentation" ) 
                                        [ documentation model dispatch ]
                            ]
                
let view model dispatch = 
     div [ ]
         [ helper
           div [ Class "tabs-container"] [ tabs model.VerificationModel dispatch ]
        //    str (model.VerifiacationModel.CurrentTab.ToString())
         ]


// let view = Ibox.btRow "Verification" ([ Ibox.emptyRow [body]
//                                         tabs ])
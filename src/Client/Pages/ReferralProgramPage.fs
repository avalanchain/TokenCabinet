module Client.ReferralProgramPage

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Elmish
open Elmish.React

open ReactBootstrap
open Helpers
open FormHelpers


// let helper2 = div [ Class "col-md-12" ] [
//     div [ Class "jumbotron" ] [ 
//         Ibox.emptyRow [
//         div [ Class "col-md-3 text-center" ] [
//                     i [ Class "fa fa-users fa-5x" ] [ ]
//                 ]
//         div [ Class "col-md-9" ] [
//             p [] [
//                 str "You can attract new users and get a bonus for them. To do this, copy the link in the box below this block. "
//             ]
//         ]
//         ]
        
//      ]
// ]

let helper = div [ Class "border-bottom ibox-content m-b-sm" ] [
                p [ Class "p-xs" ] 
                  [
                    div [ Class "pull-left m-r-md" ]
                        [ i [ Class "fa fa-globe text-info mid-icon" ] [ ] ]    
                    h2 [ ] [
                       str "Referral Program" ]
                    span [ ] [
                            str "You can attract new users and get a bonus for them. To do this, copy the link in the box below this block. "
                   ]]]
        
        // div[ Class "col-sm-10 input-group"]
let spanBtn = span [ Class "input-group-btn" ] 
                   [comF button (fun o -> o.bsClass <- Some "btn btn-success ")
                        [ strong [ ]
                            [ str "Copy" ] ] ]    



let fullInput = ((inputG (FormElement.Input InputType.Text) (None) "") @ [spanBtn])
let bodyLink = formHorizontal 
                        ([fGroupEmpty ([labelG "Link"
                                        div [ Class "col-sm-10"] 
                                            [div [ Class "input-group"] 
                                                 fullInput
                                            ]
                                            ])] 
                                            )
                        // @ [spanBtn])

let referalLink = Ibox.btRow "Referal Link" [bodyLink]


let referals = Ibox.btRow "Referals" [
                    comE table [
                        thead [][
                            tr[][
                                th [][str "#" ]
                                th [][str "Registration Date" ]
                                th [][str "Full name" ]
                                th [][str "Email" ]
                            ]
                        ]
                        tbody [][]
                    ]
                ]

let view = Ibox.emptyRow [ helper
                           referalLink
                           referals]

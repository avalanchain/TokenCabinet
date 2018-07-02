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

let helper = Ibox.btRow "Referral Program" [
        Ibox.emptyRow[
        div [ Class "col-md-1 text-center" ] [
                    i [ Class "fa fa-users fa-2x" ] [ ]
                ]
        div [ Class "col-md-11" ] [
            p [ Class "p-xss" ] [
                str "You can attract new users and get a bonus for them. To do this, copy the link in the box below this block. "
            ]
        ]]]
        
        // div[ Class "col-sm-10 input-group"]
let spanBtn = span [ Class "input-group-btn" ] [comF button (fun o -> o.bsClass <- Some "btn btn-success ")
                        [ strong [ ]
                            [ str "Copy" ] ] ]    


// labelG "Link"

let tt = div [ Class "col-sm-10"] 
               (inputG (FormElement.Input InputType.Text) "")

let fullInput = ((inputG (FormElement.Input InputType.Text) "") @ [spanBtn])
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

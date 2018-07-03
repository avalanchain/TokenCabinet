module Client.InvestmentPage

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


let helper = div [ Class "border-bottom ibox-content m-b-sm" ] [
        
        p [ Class "p-xs" ] 
          [
            div [ Class "pull-left m-r-md" ]
                [ img [ Class "w100"
                        Src "https://www.cryptocompare.com/media/20646/eth_logo.png" ] ]    
            h2 [ ] 
               [ str "Ethereum address" ]
            h4 [ Class "text-muted" ] 
               [ str "Balance" ]
            h3 [ ] [
                    str "0.0 AIMS"
           ]]]
        
        // div[ Class "col-sm-10 input-group"]
let spanBtn = span [ Class "input-group-btn" ] [comF button (fun o -> o.bsClass <- Some "btn btn-success ")
                        [ strong [ ]
                            [ str "Copy" ] ] ]    



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


let referals = Ibox.btRow "TRANSACTIONS" [
    comE table [
        thead [][
            tr[][
                th [][str "#" ]
                th [][str "Date" ]
                th [][str "Sum" ]
                th [][str "Rate" ]
                th [][str "Tokens" ]
                th [][str "Status" ]
            ]
        ]
        tbody [][]
    ]
]

let view = Ibox.emptyRow [ helper
                           referals]

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

open Shared
open ViewModels

open ReactBootstrap
open Helpers
open FormHelpers
open Fable
open CabinetModel

let bodySomeNoneCustomer model (fullCustomer: FullCustomer option)  body =
    match fullCustomer with
            | Some fc ->  body model fc 
            | None    ->  str "No model loaded" 

let helper = div [ Class "border-bottom ibox-content m-b-sm" ] [
                p [ Class "p-xs" ] 
                  [
                    div [ Class "pull-left m-r-md" ]
                        [ img [ Class "w100"
                                Src "../lib/img/coins/eth_logo.png" ] ]    
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



let fullInput = ((inputG (FormElement.Input InputType.Text) (None) "") @ [spanBtn])

let transactions = Ibox.btRow "TRANSACTIONS" [
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

let symbolAddress m = function
                    | ETH  -> m.Wallet.Accounts.Eth.Address.Value
                    | ETC  -> m.Wallet.Accounts.Etc.Address.Value
                    | BTC  -> m.Wallet.Accounts.Btc.Address.Value
                    | LTC  -> m.Wallet.Accounts.Ltc.Address.Value
                    | BCH  -> m.Wallet.Accounts.Bch.Address.Value
                    | BTG  -> m.Wallet.Accounts.Btg.Address.Value
                    | DASH -> m.Wallet.Accounts.Dash.Address.Value    
let compares (model: Model) (m: FullCustomer)  = 
    Ibox.btCol "Crypto Addresses" "12" [
            comE table [
                thead [][
                    tr[][
                        // th [ Class "text-center" ][str "Symbol" ]
                        th [ Class "text-center" ][str "Name" ]
                        th [ Class "text-center" ][str "Address" ]
                    ]
                ]
                tbody [][

                        for price in model.CurrenciesCurentPrices.Prices ->
                          tr [ ]
                               [ 
                                  td [ Class "text-center" ]
                                     [ str ( string price.Symbol) ]
                                  td [ Class "" ]
                                    [ 
                                      span [ Class "font-bold" ]
                                           [ str (symbolAddress  m price.Symbol ) ]//model
                                    ] ]
                               
                        ]
                ]
            ]
                                                       

// helper
//                                       transactions
let ccAdresses model m = bodySomeNoneCustomer model m compares

let grouped m dispatch = Ibox.emptyRow [ helper
                                         transactions]

let view (model: Model) dispatch = 
    Ibox.emptyRow [ 
        Ibox.btColEmpty "8" [ grouped model dispatch ]
        Ibox.btColEmpty "4" [ ccAdresses model model.FullCustomer ]
    ]

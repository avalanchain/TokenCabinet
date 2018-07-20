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

open ReactCopyToClipboard


// let coinbaseAccount = 
let bodySomeNoneCustomer model (fullCustomer: FullCustomer option) body dispatch=
    match fullCustomer with
            | Some fc ->  body model fc dispatch
            | None    ->  str "No model loaded" 

let bodySomeNone (model: Model) body dispatch =
    match model.TokenSale with
    | Some m ->  body model.InvestmentModel m dispatch
    | None   ->  str "No model loaded" 

let helper m mts dispatch = 
        let coinbse m dispatch = 
            if (m.Coinbase.Length = 0)
                then    
                    div []
                        [   comF button (fun o -> o.bsClass <- "btn btn-success btn-outline btn-sm" |> Some
                                                  o.onClick <- React.MouseEventHandler(fun _ -> GetCoinbase |> dispatch) |> Some  )
                                            [ 
                                                (if m.IsLoading then i [ ClassName "fa fa-circle-o-notch fa-spin" ] [] 
                                                 else str "Get Metamask Address") ]
                            h4 [ Class "text-muted" ] 
                               [ str "Balance:" ]
                        ]
                else
                div []
                    [   p [ ]
                          [ str m.Coinbase ]
                        h4 [ Class "text-muted" ] 
                               [ str "Balance:" ]
                        h3 [ ] 
                           [ str ("0.0 " + mts.SaleToken.Symbol)  ]
                        ]
               
        div [ Class "border-bottom ibox-content m-b-sm" ] [
            div [ Class "p-xs" ] 
              [
                div [ Class "pull-left m-r-md" ]
                    [ img [ Class "w100"
                            Src "../lib/img/coins/eth_logo.png" ] ]    
                h2 [ ] 
                   [ str "Ethereum address:" ]
                coinbse m dispatch
                ]]
        
        // div[ Class "col-sm-10 input-group"]
let spanBtn = span [ Class "input-group-btn" ] [comF button (fun o -> o.bsClass <- Some "btn btn-success ")
                        [ strong [ ]
                            [ str "Copy" ] ] ]    



let fullInput = ((inputG (FormElement.Input InputType.Text) (None) "") @ [spanBtn])

let transactions m dispatch = 
    let getTransactions m dispatch = 
            if (m.Coinbase.Length = 0)
                then    
                    div []
                        [   comF button (fun o -> o.bsClass <- "btn btn-success btn-outline btn-sm" |> Some
                                                  o.onClick <- React.MouseEventHandler(fun _ -> GetCoinbase |> dispatch) |> Some  )
                                            [ 
                                                (if m.IsLoading then i [ ClassName "fa fa-circle-o-notch fa-spin" ] [] 
                                                 else str "Get Metamask Address") ]
                            h4 [ Class "text-muted" ] 
                               [ str "Balance:" ]
                        ]
                else
                    tr []
                        [  
                          td [ ColSpan 6. ]  
                             [
                                 p [ Class "text-center m-t-xl" ] 
                                    [ str "no transactions" ]
                             ]
                        ]
    Ibox.btRow "TRANSACTIONS" false
        [
            comE table 
                [
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
                    tbody [ ] 
                          [
                              getTransactions m dispatch
                          ]
                ]
]

let copiedAddress (address:string) (dispatch: PurchaseTokenMsg -> unit) = 
    comF copyToClipboard (fun o ->  o.text <- address
                                    o.onCopy <- (fun (addr, b) -> addr |> AddressCopied |> dispatch) )
        [ 
            comF button (fun o -> o.bsClass <- "btn btn-success btn-outline pull-right"  |> Some )
                  [ str "Copy Address" ]
        ]        // bodyUserSomeNone
let symbolAddress m = function
                    | ETH  -> m.Wallet.Accounts.Eth.Address.Value
                    | ETC  -> m.Wallet.Accounts.Etc.Address.Value
                    | BTC  -> m.Wallet.Accounts.Btc.Address.Value
                    | LTC  -> m.Wallet.Accounts.Ltc.Address.Value
                    | BCH  -> m.Wallet.Accounts.Bch.Address.Value
                    | BTG  -> m.Wallet.Accounts.Btg.Address.Value
                    | DASH -> m.Wallet.Accounts.Dash.Address.Value    
let compares (model: Model) (m: FullCustomer) dispatch = 
    Ibox.btCol "Crypto Addresses" "12" false [
            comE table [
                thead [][
                    tr [ ][
                        th [ Class "col-md-1 text-center" ][str "Symbol" ]
                        th [ Class "col-md-1 text-center" ][str "Name" ]
                        th [ Class "col-md-9 text-center" ][str "Address" ]
                        th [ Class "col-md-11 text-center" ][str "Action" ]
                    ]
                ]
                tbody [][

                        for price in model.CurrenciesCurentPrices.Prices ->
                          tr [ ]
                               [ 
                                  td [ Class "text-center" ]
                                        [ img [ Class "w25"
                                                Src (symbolLogo price.Symbol) ] ] 
                                  td [ Class "text-center" ]
                                     [ str ( string price.Symbol) ]
                                  td [ Class "" ]
                                    [ 
                                      pre [ ]
                                           [ str (symbolAddress  m price.Symbol ) ]//model
                                    ]
                                  td [ Class "" ]
                                    [ 
                                      copiedAddress (symbolAddress  m price.Symbol) dispatch
                                    ] ]
                               
                        ]
                ]
            ]
                                                       

// helper
//                                       transactions
let ccAdresses model m dispatch= bodySomeNoneCustomer model m compares dispatch

// let grouped m mts dispatch = Ibox.emptyRow [ helper m mts dispatch
//                                          transactions]

let view (model: Model) (dispatch: Msg -> unit) = 
    Ibox.emptyRow [ 
        Ibox.btColEmptyLg "7" [ bodySomeNone model (helper)  (InvestmentsMsg >> dispatch)
                                transactions model.InvestmentModel (InvestmentsMsg >> dispatch) ]
        Ibox.btColEmptyLg "5" [ 
            Ibox.emptyRow [
                (ccAdresses model model.FullCustomer (PurchaseTokenMsg >> dispatch))] ]
    ]

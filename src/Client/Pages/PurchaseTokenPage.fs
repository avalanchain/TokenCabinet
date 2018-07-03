module Client.PurchaseTokenPage

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
open Client.CabinetModel
open Helpers
open ReactBootstrap
open FormHelpers

// let buttonToolbar = ReactBootstrap.buttonToolbar
let bodyTL m = 
      ul [ Class "timeline"
           Id "timeline" ]
           [
             for stage in m.TokenSaleStages ->
               li [ Class ("li" + (match stage.Status with 
                                    | TokenSaleStageStatus.Active       -> " active" 
                                    | TokenSaleStageStatus.Completed    -> " complete"
                                    | TokenSaleStageStatus.Cancelled    -> " cancelled"
                                    | TokenSaleStageStatus.Paused       
                                    | TokenSaleStageStatus.Expectation       -> " " )) ]
                  [ div [ Class "timestamp" ]
                      [ span [ Class "author" ]
                          [ str (stage.CapUsd.ToString() + " $") ] ]
                    div [ Class "status" ]
                      [ h4 [ ]
                          [ str (stage.CapEth.ToString() + " %") ] ] ]
           ]
let bodyRowSomeNone (model: Model) body =
    match model.TokenSale with
    | Some m -> Ibox.btRow "Timeline" ([ body m ])
    | None   -> Ibox.btRow "Timeline" ([ str "No model loaded" ]) 

let bodySomeNone (model: Model) body =
    match model.TokenSale with
    | Some m ->  body m 
    | None   ->  str "No model loaded" 

let symbolLogo = function
                    | ETH  -> "../lib/img/coins/eth_logo.png"
                    | ETC  -> "../lib/img/coins/etc_logo.png"
                    | BTC  -> "../lib/img/coins/btc_logo.png"
                    | LTC  -> "../lib/img/coins/ltc_logo.png"
                    | BCH  -> "../lib/img/coins/bch_logo.png"
                    | BTG  -> "../lib/img/coins/btg_logo.png"
                    | DASH -> "../lib/img/coins/dash_logo.png"                


let cur symbol image price isActive dispatch =
                    div [ Class "col-md-1"]
                        [ comF button (fun o -> o.bsClass <- "btn btn-default dim btn-large-dim btn-outline " + (if isActive then "active" else "") |> Some 
                                                o.onClick <- React.MouseEventHandler(fun _ -> symbol |> ActiveSymbolChanged |> PurchaseTokenMsg |> dispatch) |> Some)
                                      [ div [ Class "name" ]
                                            [str (symbol.ToString())]
                                        img [ Class "currencylogo"
                                              Src (symbolLogo symbol) ]
                                        div [ Class "price" ]
                                            [str (price.ToString() + " $")] ] ]
 
            

let bodyC (model: Model) dispatch = 
            [   for curr in model.CurrenciesCurentPrices.Prices ->
                       cur curr.Symbol curr.Symbol curr.PriceUsd (model.ActiveSymbol= curr.Symbol) dispatch
            ]


// let toggleButtonBar = comE buttonToolbar [
//                   comF (toggleButtonGroup<ToggleButtonGroup.RadioProps>) 
//                         (fun o -> 
//                              o.defaultValue <- Some (1 :> obj) //{ new System.Object() with member x.ToString() = "1"}
//                              o.name <- "options")   
//                         [
//                             // comE toggleButton [
//                             // str "Default"
//                             // ]
//                             comF toggleButton (fun o -> o.value <- U2.Case1 1. ) [
//                                 str "Default"
//                             ]
//                             comF toggleButton (fun o -> o.value <- U2.Case1 2. ) [
//                                 str "Default"
//                             ]
//                         ]   
//                 ] 

let bodyP m = div [ Class "text-center" ]
                                [ div [ Class "m-b-md" ]
                                    [ h1 [ Class "font-bold no-margins" ]
                                         [ str m.SaleToken.Symbol ]
                                        ]
                                  img [ Src "../lib/img/logo.png" //m.SaleToken.LogoUrl
                                        Class " m-b-md h90" ]
                                  h3 [ Class "font-bold no-margins" ]
                                     [ str "1 AIM = 0.02 ETH" ] 
                                  span [ Class "text-navy"]
                                       [ str ("Discount 20%" )]   ]//+ m.SaleToken.TotalSupply.ToString()

let ActiveSymbol (model: Model) =
             div[][ str (model.ActiveSymbol.ToString()) ]  

let currencies (model: Model) dispatch =  div [ Class "row seven-cols"] 
                                              (bodyC model dispatch) 
                                    
let selectedCurrency (model: Model) dispatch = 
                                    div [] 
                                        [
                                           h2 []
                                              [
                                                  str "Current:"
                                              ] 
                                           h3 []
                                              [
                                                  str " Ethereum" //model.ActiveSymbol
                                              ]
                                        ]
let currenciesGroup (model: Model) dispatch = 
        Ibox.emptyRow [ div [ Class "col-md-9" ] 
                           [ currencies model dispatch]
                        div [ Class "col-md-3" ] 
                           [ selectedCurrency model dispatch]  
                           ]

let bodyCouner m = dl [ Class "dl-horizontal" ]
                        [ dt [ ]
                            [ h4 [ ]
                                 [ str "Current discount:" ] ]
                          dd [ ]
                            [ span [ Class "label label-active" ]
                                [ str "20%" ] ]
                          br [ ]
                          dt [ ]
                             [ str "Amount:" ]
                          dd [ ]
                            [ div [ Class "col-lg-6 no-side-padding" ]
                                [ inputControl  InputType.Number
                                //   input [ Id "demo3"
                                //           Type "text"
                                //           Name "demo1" ] 
                                          ]
                              div [ Class "col-lg-6 " ]
                                [ a [ Class "btn btn-primary btn-sm" ]
                                    [ str "ACQUIRE" ] ] ]
                          br [ ]
                          dt [ ]
                            [ str ( m.SaleToken.Symbol + " :") ]
                          dd [ ]
                            [ str "300" ]
                          br [ ]
                          dt [ ]
                            [ str "Discount(AIM):" ]
                          dd [ ]
                            [ str "20.00" ] ]

let counter m = div [ Class ("col-md-9") ]
                                 [ bodySomeNone m bodyCouner  ]


let volumes m = div [ Class ("col-md-9") ]
                                 [ div [] [] ]

let counterRow m = Ibox.emptyRow [ counter m
                                   volumes m ]   

let invest m dispatch = Ibox.btCol "Invest" "9" ([ currenciesGroup m dispatch
                                                   div [ Class "hr-line-dashed" ] [ ]
                                                   counterRow m])
                                                  
let price (model: Model) = Ibox.btCol "Coin Price" "3" ([bodySomeNone model bodyP])
let secondRow m dispatch = Ibox.emptyRow [ invest m dispatch
                                           price m]
let view (model: Model) dispatch =
    div [  ]
        [ bodyRowSomeNone model bodyTL
          secondRow model dispatch
          ActiveSymbol model ]


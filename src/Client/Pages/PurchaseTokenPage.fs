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

let calcPrice activeSymbol (tick: ViewModels.CurrencyPriceTick) f =
    tick.Prices 
    |> List.tryFind(fun p -> p.Symbol = activeSymbol)
    |> f
    |> Option.defaultValue 0m

let roundFour (v:decimal) = Math.Round(v, 4)

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
    | Some m -> Ibox.btCol "Timeline" "9" ([ body m ])
    | None   -> Ibox.btCol "Timeline" "9" ([ str "No model loaded" ]) 

let bodySomeNone (model: Model) body dispatch =
    match model.TokenSale with
    | Some m ->  body m dispatch
    | None   ->  str "No model loaded" 


let bodySomeNoneTwoModels (model: Model) body dispatch =
    match model.TokenSale with
    | Some m ->  body m model.PurchaseTokenModel dispatch
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
                    span [ ]//Class "col-md-1"
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


let bodyP model  = 
    let tokenToCC = roundFour (calcPrice model.ActiveSymbol model.CurrenciesCurentPrices (Option.map2(fun (m: ViewModels.TokenSale) p -> (m.TokenSaleState.PriceUsd) / p.PriceUsd ) model.TokenSale ))
    div [ Class "text-center p-lg" ]
        [
        img [ Src "../lib/img/logo.png" 
              Class " m-b-md h90" ]
        h3 [ Class "font-bold no-margins" ]
           [ str ("1 AIM = " + tokenToCC.ToString() + " " + model.ActiveSymbol.ToString() ) ]
        span [ Class "text-navy"]
             [ str ("Discount 20%" )] ]

 
let discAr = [10, 100; 20, 200; 30, 300; 40, 400; 50, 500; 60, 600; 70, 700; 80, 800; 90, 900; 100, 1000]

let discount (m: PurchaseTokenModel) dispatch= 
      ul [ Class "timeline shift"
           Id "timeline" ]
           [
             for (per, tokens) in discAr ->
               let ttt per (tokens: decimal) = if (decimal per) > tokens then " "
                                               else "complete"
               
               li [ Class ("li " + (ttt per m.BuyTokens))]
                  [ div [ Class "timestamp" ]
                      [ span [ Class "author" ]
                          [ str (tokens.ToString() + " $") ] ]
                    div [ Class "status" ]
                      [ h4 [ ]
                          [ str (per.ToString() + " %") ] ] ]
           ]
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
                                                  str (model.ActiveSymbol.ToString()) 
                                              ]
                                        ]
let currenciesGroup (model: Model) dispatch = 
        Ibox.emptyRow [ div [ Class "col-md-9" ] 
                            (bodyC model dispatch)
                        div [ Class "col-md-3" ] 
                           [ selectedCurrency model dispatch]  
                           ]

let bodyCounter m (model:PurchaseTokenModel) dispatch = 
    dl [ Class "dl-horizontal" ]
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
            [  
                    // inputControl  InputType.Number (Some buyTokens)
                input [ 
                        Type "number" 
                        ClassName "form-control"
                        Placeholder "cryptocurrency" 
                        Min 0
                        Value (!! roundFour model.CCTokens)
                        OnChange (fun ev -> !!ev.target?value |> CCAmountChanges |> dispatch)
                        AutoFocus true ]
                // OnChange (fun ev -> dispatch (ChangeEmail !!ev.target?value))
                          
            //   div [ Class "col-lg-6 " ]
            //     [ a [ Class "btn btn-primary btn-sm" ]
            //         [ str "ACQUIRE" ] ] 
                    ]
          br [ ]
          dt [ ]
            [ str ( m.SaleToken.Symbol + " :") ]
          dd [ ]
            [ input [ 
                        Type "number" 
                        ClassName "form-control"
                        Placeholder "tokens" 
                        Min 0
                        Value (!! roundFour model.BuyTokens)
                        OnChange (fun ev -> !!ev.target?value |> TAmountChanges |> dispatch)
                         ] ]
          br [ ]
          dt [ ]
             [ str "Discount(AIM):" ]
          dd [ ]
             [ str "20.00" ] 
          br [ ]
          dt [ ]
             [ str "Total:" ]
          dd [ ]
             [ str (string (roundFour model.TotalPrice) + " $") ]]

let counter (m: Model) dispatch = div [ Class ("col-md-6") ]
                                      [ bodySomeNoneTwoModels m bodyCounter dispatch ]


// let volumes m = div [ Class ("col-md-9") ]
//                                  [ div [] [ str "sds" ] ]

let counterRow (m: Model) dispatch = Ibox.emptyRow [ counter m dispatch ]   

let invest m dispatch = Ibox.btCol "Invest" "9" ([ currenciesGroup m dispatch
                                                   div [ Class "hr-line-dashed" ] [ ]
                                                   counterRow m (PurchaseTokenMsg >> dispatch)
                                                   div [ Class "hr-line-dashed" ] [ ]
                                                   discount m.PurchaseTokenModel dispatch])


let compares = Ibox.btCol "AIM" "3" [
                    comE table [
                        thead [][
                            tr[][
                                th [ Class "text-center" ][str "Symbol" ]
                                th [ Class "text-center" ][str "Name" ]
                                th [ Class "text-center" ][str "Account" ]
                            ]
                        ]
                        tbody [][
                                 tr [ ]
                                    [ td [ Class "text-center" ]
                                        [ img [ Class "w25"
                                                Src "../lib/img/coins/eth_logo.png" ] ]
                                      td [ Class "text-center" ]
                                        [ str "ETH" ]
                                      td [ Class "text-right" ]
                                        [ span [ Class "text-info font-bold" ]
                                            [ str "1125.00 " ]
                                          img [ Alt "image"
                                                Class "w25"
                                                Src "../lib/img/logo.png" ] ] ]
                                 tr [ ]
                                    [ td [ Class "text-center" ]
                                        [ i [ Class "fa fa-btc fa-2x" ]
                                            [ ] ]
                                      td [ Class "text-center" ]
                                        [ str "BTC" ]
                                      td [ Class "text-right" ]
                                        [ span [ Class "text-info font-bold" ]
                                            [ str "11532.00 " ]
                                          img [ Alt "image"
                                                Class "w25"
                                                Src "../lib/img/logo.png" ] ] ]
                                 tr [ ]
                                    [ td [ Class "text-center" ]
                                        [ i [ Class "fa fa-usd fa-2x" ]
                                            [ ] ]
                                      td [ Class "text-center" ]
                                        [ str "USD" ]
                                      td [ Class "text-right" ]
                                        [ span [ Class "text-info font-bold" ]
                                            [ str "5.20 " ]
                                          img [ Alt "image"
                                                Class "w25"
                                                Src "../lib/img/logo.png" ] ] ]
                        ]
                    ]
                ]                                                
let price (model: Model) = Ibox.btColContentOnly "3" ([ bodyP model ])

let firstRow m dispatch = Ibox.emptyRow [ bodyRowSomeNone m bodyTL
                                          price m ]
let secondRow m dispatch = Ibox.emptyRow [ invest m dispatch
                                           compares ]
let view (model: Model) dispatch =
    div [  ]
        [ firstRow model dispatch
          secondRow model dispatch ]


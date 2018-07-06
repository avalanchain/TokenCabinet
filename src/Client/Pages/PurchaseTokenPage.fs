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
open Shared.WalletPublic

// let buttonToolbar = ReactBootstrap.buttonToolbar
importAll "../../../node_modules/qrcode.react/lib/index.js"
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

let bodySomeNone (model: Model) body =
    match model.TokenSale with
    | Some m ->  body m 
    | None   ->  str "No model loaded" 

let bodySomeNoneTwoModels (model: Model) body dispatch =
    match model.TokenSale with
    | Some m ->  body m model.PurchaseTokenModel dispatch
    | None   ->  str "No model loaded" 

let cur symbol image price isActive dispatch =
                    span [ ]//Class "col-md-1"
                        [ comF button (fun o -> o.bsClass <- "crypto btn btn-default dim btn-large-dim btn-outline " + (if isActive then "active" else "") |> Some 
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
        h2 [ Class "no-margins" ]
           [ str ("1 AIM = " + tokenToCC.ToString() + " " + model.ActiveSymbol.ToString() ) ]
        span [ Class "text-navy"]
             [ str ("Discount 20%" )] ]

 
let discAr = [10, 100; 20, 200; 30, 300; 40, 400; 50, 500; 60, 600; 70, 700; 80, 800; 90, 900; 100, 1000]

let bonus (m: PurchaseTokenModel) dispatch= 
      ul [ Class "timeline shift"
           Id "timeline" ]
           [
             for (per, tokens) in discAr ->
               let ttt per (tokens: decimal) = if (decimal per) > tokens then " "
                                               else "complete"
               
               li [ Class ("li " + (ttt per m.BuyTokens))]
                  [ div [ Class "timestamp" ]
                      [ span [ Class "author" ]
                          [ str (tokens.ToString()) ] ]
                    div [ Class "status" ]
                      [ h4 [ ]
                          [ str (per.ToString() + " %") ] ] ]
           ]
// [<Pojo>]
// type QrCodeProps =
//   { 
//       value : string
//   }

// let qrCodeProps:QrCodeProps = {
//     value = "avalanchain.com"
// }

// let getAddress account activeSymbol = 
//     if(account=activeSymbol)
            
let qrCode address = ofImport "default" "qrcode.react" (createObj [ "value" ==> address 
                                                                    "size" ==> 100 ]) []
                                    
let selectedCurrency (model: Model) dispatch = 
        div [] 
            [
               h2 []
                  [ str ("Current " + model.ActiveSymbol.ToString() + " Address:") ] 
               p []
                  [ str ( model.PurchaseTokenModel.CCAddress) ]
            ]
            // bodyUserSomeNone
let currenciesGroup (model: Model) dispatch = 
        Ibox.emptyRow [ div [ Class "col-md-8" ] 
                            (bodyC model dispatch)
                        div [ Class "col-md-4" ] 
                           [ selectedCurrency model dispatch
                             qrCode model.PurchaseTokenModel.CCAddress]  
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
             [ 
                div [ Class "col-lg-6 " ]
                    [ 
                       h3 [ ]
                          [
                              str (string (roundFour model.TotalPrice) + " $")
                          ] 
                     ] 
                div [ Class "col-lg-6 " ]
                    [ a [ Class "btn btn-primary btn-sm" ]
                        [ str "BUY TOKENS" ] ] 
                  ]]

let counter (m: Model) dispatch = div [ Class ("col-md-6") ]
                                      [ bodySomeNoneTwoModels m bodyCounter dispatch ]


let tokenSale (m:ViewModels.TokenSale) = 
    div [Class "col-md-4 col-md-offset-1"]
        [
         ul [ Class "stat-list" ]
            [ li [ ]
                [ h2 [ Class "no-margins " ]
                    [ str (m.SoftCapEth.ToString() + " ETH" ) ]
                  small [ ]
                    [ str "SoftCap" ]
                  div [ Class "stat-percent" ]
                    [ str "52%"
                      i [ Class "fa fa-bolt text-navy" ]
                        [ ] ]
                  div [ Class "progress progress-mini" ]
                    [ div [ 
                        // HTMLAttr.Custom ("style", "width: 52%;")
                            Class "progress-bar-success" ]
                        [ ] ] ]
              li [ ]
                [ h2 [ Class "no-margins" ]
                    [ str (m.HardCapEth.ToString() + " ETH" ) ]
                  small [ ]
                    [ str "HardCap" ]
                  div [ Class "progress progress-mini" ]
                    [ div [ 
                        // HTMLAttr.Custom ("style", "width: 48%;")
                            Class "progress-bar" ]
                        [ ] ] ]
              li [ ]
                [ h2 [ Class "no-margins " ]
                    [ str (m.Expectations.ToString() + " ETH" ) ]
                  small [ ]
                    [ str "Expectations" ]
                  div [ Class "progress progress-mini" ]
                    [ div [ 
                        // HTMLAttr.Custom ("style", "width: 60%;")
                            Class "progress-bar-success" ]
                        [ ] ] ] ]

        ]
// let volumes m = div [ Class ("col-md-9") ]
//                                  [ div [] [ str "sds" ] ]

let bounusCommon ptm dispatch = bonus ptm dispatch
    // div [ Class "row" ]
    //     [ 
    //         bonus ptm dispatch
    //     ]

let counterRow (m: Model) dispatch = Ibox.emptyRow [ counter m dispatch 
                                                     bodySomeNone m tokenSale]   

let invest m dispatch = Ibox.btCol "Invest" "9" ([ currenciesGroup m dispatch
                                                   div [ Class "hr-line-dashed" ] [ ]
                                                   counterRow m (PurchaseTokenMsg >> dispatch)
                                                   div [ Class "hr-line-dashed" ] [ ]
                                                   bounusCommon m.PurchaseTokenModel dispatch])
//compares not used 
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
                                            [ str "185.00 " ]
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
                                            [ str "1532.00 " ]
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


let timelineItem (stage:TokenSaleStage) =
     div [ Class ("timeline-item " + if stage.Status = TokenSaleStageStatus.Active then "active" else "") ] 
         [ 
             Ibox.emptyRow 
                [   div [ Class "col-xs-4 date" ] 
                        [
                           i [ Class ("fa " + (match stage.Status with 
                                                | TokenSaleStageStatus.Active       -> "fa-play text-navy" 
                                                | TokenSaleStageStatus.Completed    -> "fa-check-square-o text-navy"
                                                | TokenSaleStageStatus.Cancelled    -> "fa-close "
                                                | TokenSaleStageStatus.Paused       -> "fa-pause"
                                                | TokenSaleStageStatus.Expectation  -> "fa-clock-o " )) ][]
                           div [  ]
                               [ 
                                   span [ Class "font-bold" ] 
                                        [ str "Start Date: "]

                                   span [ Class "border-bottom" ] 
                                        [ str (stage.StartDate.ToShortDateString())]
                                ]        
                           div [ Class "m-t-sm" ]
                               [   
                                   span [ Class "font-bold" ] 
                                        [ str "End Date: "]

                                   span [ Class "border-bottom"  ] 
                                        [ str (stage.EndDate.ToShortDateString())]
                               ]             
                         ]
                    div [ Class "col-xs-7 content" ] //no-top-border if first 
                        [
                            div [ Class "m-b-xs" ]
                                [
                                  h3 [ Class ("" + if stage.Status = TokenSaleStageStatus.Active then "" else "text-info") ] 
                                         [ str stage.Name ]
                                ]
                            div [ Class "m-b-xs" ]
                              [
                                  h3 [ Class ""  ] 
                                      [ str ( string stage.Status)]

                                  div [ Class "m-t-sm"  ] 
                                       [ str "Cap: " 
                                         span [ Class "font-bold" ] 
                                              [ str ((string stage.CapEth) + " ETH")] ]

                                  div [ Class "m-t-sm m-b-sm"  ] 
                                       [ str "Cap USD: "
                                         span [ Class "font-bold" ] 
                                              [ str ((string stage.CapUsd) + " $")] ]
                              ]
                         ]
                ]    
         ]
let stages m = Ibox.btColEmpty "3" 
                [
                    Ibox.iboxTitle "Token Sale"
                    // Ibox.iboxContentOnly2 [
                    //     div [ ] 
                    //         [
                    //             h3 [ ]
                    //                [ str ("Active Satge: " + m.TokenSaleState.ActiveStage.Name ) ]
                    //             div [ Class "row" ]
                    //                [   
                    //                    div [ Class "col-md-6" ]
                    //                        [ 
                    //                            span [ Class "font-bold" ] 
                    //                                 [ str "Started: "]

                    //                            span [ Class "border-bottom" ] 
                    //                                 [ str (m.TokenSaleState.ActiveStage.StartDate.ToShortDateString())]
                    //                         ]        
                    //                    div [ Class "col-md-6" ]
                    //                        [   
                    //                            span [ Class "font-bold" ] 
                    //                                 [ str "End Date: "]

                    //                            span [ Class "border-bottom"  ] 
                    //                                 [ str (m.TokenSaleState.ActiveStage.EndDate.ToShortDateString())]
                    //                        ]   ] 
                    //         ]] "ibox-heading"
                    Ibox.iboxContentOnly2 [ 
                        div [ ] 
                            [ 
                                for stage in m.TokenSaleStages ->
                                timelineItem stage ]] ""
                ]
let price (model: Model) = Ibox.btColContentOnly "3" ([ bodyP model ])
let columnFirstRow m = div []
                         [ 
                           price m
                           bodySomeNone m stages
                           ]
let firstRow m dispatch = Ibox.emptyRow [ invest m dispatch 
//bodyRowSomeNone m bodyTL
                                          columnFirstRow m ]
// let secondRow m dispatch = Ibox.emptyRow [ 
//                                            compares ]
let view (model: Model) dispatch =
    div [  ]
        [ firstRow model dispatch ]


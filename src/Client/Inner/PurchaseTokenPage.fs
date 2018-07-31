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
open Cabinet
open CabinetModel
open Helpers
open ReactBootstrap
open FormHelpers
open Shared.WalletPublic
open Client
open ReactCopyToClipboard
open ReactBootstrap.ProgressBar


importAll "../../../node_modules/react-rangeslider/lib/index.css"
importAll "../../Client/lib/css/inspinia/flatUIRange.css"
// let buttonToolbar = ReactBootstrap.buttonToolbar
importAll "../../../node_modules/qrcode.react/lib/index.js"
importAll "../../../node_modules/react-moment/dist/index.js"
importAll "../../../node_modules/react-rangeslider/lib/Rangeslider.js"
importAll "../../../node_modules/react-rangeslider/lib/index.js"


// importAll "../../../node_modules/ion-rangeslider/css/ion.rangeSlider.skinFlat.css"
// importAll "../../../node_modules/ion-rangeslider/js/ion.rangeSlider.min.js"

let calcPrice activeSymbol (tick: ViewModels.CurrencyPriceTick) f =
    tick.Prices 
    |> List.tryFind(fun p -> p.Symbol = activeSymbol)
    |> f
    |> Option.defaultValue 0m

let bodyRowSomeNone (model: Model) body =
    match model.TokenSale with
    | Some m -> Ibox.btCol "Timeline" "9" false ([ body m ])
    | None   -> Ibox.btCol "Timeline" "9" false ([ str "No model loaded" ]) 

let bodySomeNone (model: Model) body =
    match model.TokenSale with
    | Some m ->  body m 
    | None   ->  str "No model loaded" 

let bodySomeNoneTwoModels (model: Model) body dispatch =
    match model.TokenSale with
    | Some m ->  body model.ActiveSymbol model.IsWeb3 m model.PurchaseTokenModel dispatch
    | None   ->  str "No model loaded" 

let cur symbol image price isActive dispatch =
                    span [ ]//Class "col-md-1"
                        [ comF button (fun o -> o.bsClass <- "crypto btn btn-default dim btn-large-dim btn-outline " + (if isActive then "active btn-green" else "") |> Some 
                                                o.onClick <- React.MouseEventHandler(fun _ -> symbol |> ActiveSymbolChanged |> PurchaseTokenMsg |> dispatch) |> Some)
                                      [ div [ Class "name" ]
                                            [str (symbol.ToString())]
                                        img [ Class "currencylogo"
                                              Src (symbolLogo symbol) ]
                                        div [ Class "price" ]
                                            [str (price.ToString() + " $")] ] ]
 
let rMoment (date:DateTime)  = ofImport "default" "react-moment" (createObj [  "date" ==> date
                                                                               "fromNow" ==> true  ]) []

let bodyCurrenciesTime (m: Model) dispatch = 
            div [ Class "m-b-sm" ] 
                [ 
                  small [ Class "label bg-muted"  ]
                        [
                          i [ Class "fa fa-clock-o" ] [ ]
                          str " Prices Updated: " 
                          (if m.CurrenciesCurentPrices.Prices.IsEmpty then str " " else rMoment m.CurrenciesCurentPrices.Prices.Head.PriceAt )
                          //str (" Updated: " + m.CurrenciesCurentPrices.Prices.Head.PriceAt.ToShortTimeString()
                         ] 
                    

                   
                ]         

let bodyCurrencies (model: Model) dispatch = 
            [   for curr in model.CurrenciesCurentPrices.Prices ->
                       cur curr.Symbol "" curr.PriceUsd (model.ActiveSymbol = curr.Symbol) dispatch
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


let discountArray = [|1000m, 10m; 2000m, 20m; 3000m, 30m; 4000m, 40m; 5000m, 50m; 6000m, 60m; 7000m, 70m; 8000m, 80m; 9000m, 90m; 10000m, 100m|]
let rised = 32324
let bonus (m: PurchaseTokenModel) = 
      div [ Class "col-sm-11" ]
          [ ul [ Class "timeline shift"
                 Id "timeline" ]
               [
                 for (boundary, percent) in discountArray ->
                   let isComplete = if boundary > m.BuyTokens then " "
                                    else "complete"
                   
                   li [ Class ("li " + isComplete )]
                      [ div [ Class "timestamp" ]
                          [ span [ Class "author" ]
                              [ str (boundary.ToString()) ] ]
                        div [ Class "status" ]
                          [ h4 [ ]
                              [ str (percent.ToString() + " %") ] ] ]
               ]]
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
               pre [ Class "text-center" ]
                  [ str ( model.PurchaseTokenModel.CCAddress) ]
            ]


let copiedAddress (address:string) (dispatch: PurchaseTokenMsg -> unit) = 
    comF copyToClipboard (fun o ->  o.text <- address
                                    o.onCopy <- (fun (addr, b) -> addr |> AddressCopied |> dispatch) )
        [ 
            comF button (fun o -> o.bsClass <- "btn btn-success btn-outline pull-right"  |> Some )
                  [ str "Copy Address" ]
        ]        // bodyUserSomeNone
let currenciesGroup (model: Model) dispatch = 
        Ibox.emptyRow [ div [ Class "col-md-7" ] 
                            [ 
                                bodyCurrenciesTime model dispatch
                                div [ ] 
                                    (bodyCurrencies model dispatch) 
                            ]
                        div [ Class "col-md-5" ] 
                           [ selectedCurrency model dispatch
                             Ibox.emptyRow 
                                [
                                    Ibox.btColEmpty "6" [ qrCode model.PurchaseTokenModel.CCAddress ]
                                    Ibox.btColEmpty "6" [ copiedAddress model.PurchaseTokenModel.CCAddress (PurchaseTokenMsg >> dispatch) ]
                                ]
                            
                            ]  
                            ]

let bodyCounter activeSymbol isWeb3 m (model:PurchaseTokenModel) dispatch = 
    let isHide = if (activeSymbol = CryptoCurrencySymbol.ETH && (not isWeb3) && model.CCTokens <> 0m && model.BuyTokens <> 0m) then "" else "hidden" 
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
             [ str "Discount:" ]
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
                    [   
                        comF button (fun o -> o.bsClass <- "btn btn-primary full-width btn-sm " + isHide |> Some
                                              o.onClick <- React.MouseEventHandler(fun _ -> BuyTokens |> dispatch) |> Some  )
                                    [ 
                                        (if model.IsLoading then i [ ClassName "fa fa-circle-o-notch fa-spin" ] [] 
                                         else str "BUY TOKENS") ]
                        
                           
                           ] 
                  ]]

let counter (m: Model) dispatch = div [ Class ("col-md-6") ]
                                      [ bodySomeNoneTwoModels m bodyCounter (dispatch) ]


///TODO: add jquery slider range with flat ui
// let jq = importDefault<obj> "jquery"
// let go = jq $ ("#go")

// let rangeTest volume =
let range volume = ofImport "default" "react-rangeslider" 
                            (createObj [  "value" ==> volume 
                                          "min" ==> 0
                                          "max" ==> 60_000
                                          "from" ==> 200
                                          "to" ==> 12104
                                          "type" ==> "double" 
                                          "prefix" ==> "$" 
                                          ]) []
let tokenSale (m:ViewModels.TokenSale) = 
    div [ Class "col-md-5 col-md-offset-1" ]
        [
         ul [ Class "stat-list" ]
            [  li [ Class "row" ]
                [ 
                  div [ Class "col-md-6" ] 
                    [ 
                        h2 [ Class "no-margins " ]
                           [ str (m.SoftCapEth.ToString() + " ETH" ) ]
                        small [ ]
                            [ str "SoftCap" ]
                        div [ Class "stat-percent" ]
                            [ str "52%"
                              i [ Class "fa fa-bolt text-navy" ]
                                [ ] ]
                        div [ Class "progress progress-mini" ]
                            [ 
                                comF progressBar (fun p -> p.now <- 100. |> Some
                                                           p.bsClass <- " progress-bar-success"  |> Some    )
                                                 []
                                // div [ 
                                // // HTMLAttr.Custom ("style", "width: 52%;")
                                //     Class "progress-bar-success" ]
                                //  [ ] 
                                 ] 
                    ]
                  div [ Class "col-md-6" ] 
                    [ 
                        h2 [ Class "no-margins" ]
                            [ str (m.HardCapEth.ToString() + " ETH" ) ]
                        small [ ]
                            [ str "HardCap" ]
                        div [ Class "progress progress-mini" ]
                            [ 
                                comF progressBar (fun p -> p.now <- 60. |> Some)
                                                 []
                                // div [ 
                                // // HTMLAttr.Custom ("style", "width: 52%;")
                                //     Class "progress-bar-success" ]
                                //  [ ] 
                                 ]
                    ]
                 
                        
                        
                        ]
            //    li [ ] 
            //     [
                    
            //      ]       
               li [ ]
                [ 
                  div [ ]
                      [
                          h2 [ Class "no-margins" ]
                             [ str (string rised + " ETH")  ]
                          small [ ]
                            [ str "Rised" ]
                          div []
                                [
                                    range rised
                                ] 
                      ]    
                    
                  ]

                //   div [ Class "col-lg-offset-6 col-lg-6" ]
                //       [
                //           h2 [ Class "no-margins" ]
                //              [ str " 1232434 ETH"  ]
                //           small [ ]
                //             [ str "Counted" ]
                         
                //       ]    
                //   div [ Class " col-lg-12" ]
                //       [
                //           range 1232434
                //       ] 
            //   li [ ]
            //     [ h2 [ Class "no-margins " ]
            //         [ str (m.Expectations.ToString() + " ETH" ) ]
            //       small [ ]
            //         [ str "Expectations" ]
            //       div [ Class "progress progress-mini" ]
            //         [ div [ 
            //             // HTMLAttr.Custom ("style", "width: 60%;")
            //                 Class "progress-bar-success" ]
            //             [ ] ] ] 
                        
                        ]

        ]
// let volumes m = div [ Class ("col-md-9") ]
//                                  [ div [] [ str "sds" ] ]

let bounusLeft (tokenSale:TokenSale) = 
    
    div [ Class "col-sm-1" ]
        [ 
            h3 [ ]
                [ str (string tokenSale.SaleToken.Symbol)]
            hr []
            h3 [ ]
               [ str "Bonus" ]
        ]

let countBonus (buyTokens:decimal) = 
        discountArray
        |> Array.tryFindBack (fun (boundary, value) -> buyTokens >= boundary)
        |> Option.defaultValue (0m, 0m)
        |> fun (_, percent) -> buyTokens * percent / 100m 
        // |> Array.find (fun (bound, value) -> buyTokens <= bound)
        // |> fst  
            //|> Map.filter (fun percents _ -> (decimal (percents * 100)) > buyTokens  )

                // for (percents, tokens) in discountArray ->
                //    let isComplete percents (tokens: decimal) = if (decimal (percents * 100)) > tokens then " "
                //                                                else percents * 100
                // isComplete percents buyTokens                                         
                   
                   
let totalCoins (m: Model) = 
    div [ Class "col-md-5 col-md-offset-1" ]
        [
            h2 [ ]
               [
                    str ( "Tokens: " + string (roundFour m.PurchaseTokenModel.BuyTokens))
                    span [ Class "text-navy pull-right" ]
                       [
                            str ( " Bonus: " + string (roundFour (countBonus m.PurchaseTokenModel.BuyTokens)))
                        ]  
               ]
            h1 [ ]
               [
                   span [ Class "pull-right font-bold" ]
                       [
                    str ( " Total: " + string (roundFour ( m.PurchaseTokenModel.BuyTokens + (countBonus m.PurchaseTokenModel.BuyTokens))))
                    
                    ]
               ] 
        ]

let counterRow (m: Model) dispatch = Ibox.emptyRow [ counter m dispatch 
                                                     bodySomeNone m tokenSale
                                                     totalCoins m ]   

let invest m dispatch = 
    Ibox.btColLg "Invest" "9" m.PurchaseTokenModel.IsLoading 
        ([ currenciesGroup m dispatch
           div [ Class "hr-line-dashed" ] [ ]
           counterRow m (PurchaseTokenMsg >> dispatch)
           div [ Class "hr-line-dashed" ] [ ]
           Ibox.emptyRow 
              [
                 bodySomeNone m bounusLeft
                 bonus m.PurchaseTokenModel
              ]
            ])                                               


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
let stages m = Ibox.btColEmptyLg "3" 
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
let price (model: Model) = Ibox.btColContentOnlyLg "3" ([ bodyP model ])
let columnFirstRow m = div []
                         [ 
                           price m
                           bodySomeNone m stages
                           ]
let firstRow m dispatch = Ibox.emptyRow [ invest m dispatch 
                                          columnFirstRow m ]

let view (model: Model) (dispatch: Msg -> unit) =
    div [  ]
        [ firstRow model dispatch ]


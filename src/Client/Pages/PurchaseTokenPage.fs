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

let cur name image price =
                    div [ Class "col-md-1"]
                        [ comF button (fun o -> o.bsClass <- Some "btn btn-default dim btn-large-dim btn-outline") 
                                      [ div [ Class "name" ]
                                            [str (name.ToString())]
                                        img [ Class "currencylogo"
                                              Src "https://www.cryptocompare.com/media/20646/eth_logo.png" ]
                                        div [ Class "price" ]
                                            [str (price.ToString() + " $")] ] ]
 
            

let bodyC (model: Model) = 
            [   for curr in model.CurrenciesCurentPrices.Prices ->
                       cur curr.Symbol curr.Symbol curr.PriceUsd
            ]


let test = comE buttonToolbar [
                  comF (toggleButtonGroup<ToggleButtonGroup.RadioProps>) 
                        (fun o -> 
                             o.defaultValue <- Some (1 :> obj) //{ new System.Object() with member x.ToString() = "1"}
                             o.name <- "options")   
                        [
                            // comE toggleButton [
                            // str "Default"
                            // ]
                            comF toggleButton (fun o -> o.value <- U2.Case1 1. ) [
                                str "Default"
                            ]
                            comF toggleButton (fun o -> o.value <- U2.Case1 2. ) [
                                str "Default"
                            ]
                        ]   
                ] 

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

// let priceBd (model: Model) =
//     match model.TokenSale with
//     | Some m -> [ bodyP m ]
//     | None   -> [ str "No model loaded" ] 

let currencies (model: Model) =  div [ Class "row seven-cols"] (bodyC model) 

let bodyCouner m = dl [ Class "dl-horizontal" ]
                        [ dt [ ]
                            [ h4 [ ]
                                 [ str "Current discount:" ] ]
                          dd [ ]
                            [ span [ Class "label label-active" ]
                                [ str "12.5%" ] ]
                          br [ ]
                          dt [ ]
                             [ h4 [ ]
                                  [ str "Amount:" ] ]
                          dd [ ]
                            [ div [ Class "col-lg-6 no-side-padding" ]
                                [ input [ Id "demo3"
                                          Type "text"
                                          Name "demo1" ] ]
                              div [ Class "col-lg-6 " ]
                                [ a [ Class "btn btn-danger btn-sm" ]
                                    [ str "ACQUIRE" ] ] ]
                          br [ ]
                          dt [ ]
                            [ h4 [ ]
                                 [ str ( m.SaleToken.Symbol + " :") ] ]
                          dd [ ]
                            [ str "300" ]
                          br [ ]
                          dt [ ]
                            [ str "Discount(AIM):" ]
                          dd [ ]
                            [ str "12,5" ] ]

let counter m = div [ Class ("col-md-9") ]
                                 [ bodySomeNone m bodyCouner  ]


let volumes m = div [ Class ("col-md-9") ]
                                 [ div [] [] ]

let counterRow m = Ibox.emptyRow [ counter m
                                   volumes m ]   

let invest m = Ibox.btCol "Invest" "9" ([ currencies m
                                          counterRow m])
                                                  
let price (model: Model) = Ibox.btCol "Coin Price" "3" ([bodySomeNone model bodyP])
let secondRow m = Ibox.emptyRow [ invest m
                                  price m]
let view (model: Model) =
    div [  ]
        [ bodyRowSomeNone model bodyTL
          secondRow model
          test]


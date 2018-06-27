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
let timeline (model: Model) =
    match model.TokenSale with
    | Some m -> Ibox.viewRow "Timeline" ([ bodyTL m ])
    | None   -> Ibox.viewRow "Timeline" ([ str "No model loaded" ]) 

let cur name image price =
            button [ Class "btn btn-info  dim btn-large-dim btn-outline"
                     Type "button" ]
                    [  div [ Class "name" ]
                            [str name]
                       img [ Class "currencylogo"
                             Src "https://www.cryptocompare.com/media/20646/eth_logo.png" ]
                       div [ Class "price" ]
                           [str (price + " $")] ]
let currenciesdata = ["ETH", "img", "750";"ETH", "img", "750";"ETH", "img", "750"]
let bodyC  = 
            [   for (name, img, price) in currenciesdata ->
                    cur name img price
            ]
let currencies (model: Model) = Ibox.viewRow "Currencies" bodyC

let view (model: Model) =
    div [  ]
        [ timeline model
          currencies model]


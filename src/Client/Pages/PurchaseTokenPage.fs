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


let body m = 
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
    | Some m ->
        Ibox.viewRow "Timeline" (body m)
    | None -> Ibox.viewRow "Timeline" (div [ ] [str "No model loaded" ]) 




let view (model: Model) =
    div [  ]
        [ timeline model ]


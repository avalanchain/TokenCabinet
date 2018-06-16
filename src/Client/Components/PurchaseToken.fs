module Client.PurchaseToken

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open Fable
open Shared.ViewModels
open Fable.DateFunctions

open System
open Client.Helpers


let info (model : Model) (dispatch : Msg -> unit) =
    let fieldPairs = 
        match model.TokenSale with
            | Some v -> [   "Sale Start Date", v.StartDate.ToShortDateString()
                            "Sale End Date"  , v.EndDate.ToShortDateString()
                            "Soft Cap USD"   , v.SoftCapUsd.ToString()
                            "Hard Cap USD"   , v.HardCapUsd.ToString() ]
            | None -> [ "", "" ]
    fieldPairs |> toTiles       

let currencies (model : Model) (dispatch : Msg -> unit) =
    let fieldPairs = 
        [ for price in model.CurrenciesCurentPrices.Prices -> price.Symbol, price.PriceUsd.ToString() ]
    fieldPairs |> Helpers.toTiles   

let tokenSaleStages  (model : Model) (dispatch : Msg -> unit) =

    let convertDateTime (dt : DateTime) = dt.ToShortDateString()
    let getStatus = function    
                        | Expectation -> ""
                        | Active -> "is-active"
                        | Completed -> " is-completed is-success"
                        | Cancelled -> ""
                        | Paused -> ""
    let getIcon = function    
                            | Expectation -> ""
                            | Active -> ""
                            | Completed -> "fa-check"
                            | Cancelled -> ""
                            | Paused -> ""

    model.TokenSale.Value.TokenSaleStages 
    |> List.mapi (fun idx a -> div [ Class ("step-item  " + (getStatus a.Status)) ]
                                [ div [ Class "step-marker" ]
                                    [ span [ Class "icon" ]
                                        [ i [ Class ("fa " + getIcon a.Status) ]
                                            [ ] ] ]
                                  div [ Class "step-details" ]
                                    [ p [ Class "step-title" ]
                                        [ str (string a.Name)  ]
                                      p [ ]
                                        [ str ( string a.CapEth + " ETH") ] 
                                      p [ Class "is-size-7" ]
                                        [ str ( convertDateTime a.StartDate.Date + " - " + convertDateTime a.EndDate) ]] ] )
    |> div [ Class "steps is-medium"] 
    


let purchaseTokenView  (model : Model) (dispatch : Msg -> unit) = 
    div [ ]
        [   HeroTile.hero
            tokenSaleStages   model dispatch
            info              model dispatch
            currencies        model dispatch
          ]


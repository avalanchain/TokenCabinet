module Client.PurchaseTokenView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.DateFunctions
open Fulma
open ClientModelMsg
open Fable

open Fable.Core
open Shared.ViewModels

open System
open Client.Helpers
open Fable.Core.JsInterop
open ReactChartJs2
let formatOptions = createEmpty<IDistanceInWordsOptions>
formatOptions.includeSeconds <- false
formatOptions.addSuffix <- true
formatOptions.locale <- DateTime.Locales.Russian

let info (model : Model) (dispatch : Msg -> unit) =
    let fieldPairs = 
        match model.TokenSale with
            | Some v -> [   "Sale Start Date", ExternalDateFns.formatWithStr v.StartDate "Do MMM YYYY" 
                            "Sale End Date"  , ExternalDateFns.formatWithStr v.EndDate "Do MMM YYYY" 
                            "Soft Cap USD"   , v.SoftCapUsd.ToString()
                            "Hard Cap USD"   , v.HardCapUsd.ToString() ]
            | None -> [ "", "" ]
    fieldPairs |> toTiles       

let currencies (model : Model) (dispatch : Msg -> unit) =
    let fieldPairs = 
        [ for price in model.CurrenciesCurentPrices.Prices -> price.Symbol, price.PriceUsd.ToString()+ " $" ]
    fieldPairs |> Helpers.toTiles   

let tokenSaleStages  (model : Model) (dispatch : Msg -> unit) =

    let convertDateTime (dt : DateTime) = ExternalDateFns.formatWithStr dt "Do MMM YYYY" 
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
 //////Chart zone   
let datasets = jsOptions<ChartJs.Chart.ChartDataSets>(fun o -> 
    o.data <- [| 300.; 50.; 100. |] |> U2.Case1 |> Some
    o.backgroundColor <- [| "#23d160"; "#00D1B2"; "#b5b5b5" |] |> Array.map U4.Case1 |> U2.Case2 |> Some
    o.hoverBackgroundColor <- [| "#23d160";  "#00D1B2"; "#b5b5b5" |] |> U2.Case2 |> Some
)

let chartJsData: ChartJs.Chart.ChartData = {
    labels = [| "Completed"; "Active"; "Waiting" |] |> Array.map U2.Case1  
    datasets = [| datasets |] 
}

let chartProps = jsOptions<ChartComponentProps>(fun o -> 
    o.data <- chartJsData |> ChartData.ofT );

let chart = div [ Id "doughnut-card"
                  Class "flex-card light-bordered card-overflow light-raised" ]
                [ h3 [ Class "card-heading is-absolute" ]
                     [ str "ICO progress" ]
                  
                  ofImport "Doughnut" "react-chartjs-2" chartProps []
                  
                  div [ Class "has-text-centered mt-50" ]
                    [ a [ Class "button btn-dash secondary-btn btn-dash is-raised rounded ripple"
                          HTMLAttr.Custom ("data-ripple-color", "") ]
                        [ str "See all data" ] ] ]
 //////Chart zone  end 
let view  (model : Model) (dispatch : Msg -> unit) = 
    div [ Class "dashboard-wrapper" ]
        [   HeroTile.hero
            tokenSaleStages   model dispatch
            info              model dispatch
           
            div [ Class "columns"]
                [
                    div [ Class "column is-9"]
                        [ currencies model dispatch]
                    div [ Class "column is-3"]
                        [ chart ]
                ]
          ]


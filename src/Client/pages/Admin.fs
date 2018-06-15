module Client.Admin

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open Elmish
open Elmish.React
open Elmish.React.Common

open ServerCode
open ServerCode.Domain
open ServerCode.Commodities
open ServerCode.EntityList
open ServerCode.Utils

open Utils
open Entity
open Style
open Messages

open Client.Trading

type Page =
    | Contracts

let init (user: UserData) (sp: Page) = 
   { Contracts = Map.empty }, Cmd.none


let update (msg: AdminMsg) model : Model * Cmd<AdminMsg> = 
    Utils.delete "ContractAddress"
    Browser.console.log "Local storage entry 'ContractAddress' deleted"
    Browser.location.reload true
    { Contracts = Map.empty }, Cmd.none

open Fable.Import.BootstrapTable
 
let view (model: Model) (dispatch: AdminMsg -> unit) = 
    [   div [ ClassName "row" ] [
            h4 [ ClassName "col-6" ] [ text "All Contracts as JSON" ]
            div [ ClassName "col-6 float-right" ] [ 
                button [ClassName ("btn btn-sm btn-danger") 
                        OnClick (fun _ -> dispatch Reload)
                ] [ text "RESET Contracts" ]
             ]
        ]
        
        hr []
        div [ ClassName "card"] [
            div [ ClassName "card-block"] [
                // div [ ClassName "row"] [
                //     div [ ClassName "col-sm-12"] [
                //         h4 [ ClassName "card-title mb-0"] [ text "AdMiN" ]
                //     ]
                // ]
                div [ ClassName "row"] [
                    div [ ClassName "col-sm-12"] [
                        pre [] [ model |> toJsonPretty 2. |> text ]
                    ]
                ]
            ]
        ]
        hr []
    ] |> div [] 
        

module Client.Home

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Client.Style
open ServerCode.Utils
open ServerCode.Commodities

open ReactStrap
open ReactJsonSchemaForm
open ReactDatePicker
open Fable.Helpers.Moment

module RSel = Fable.Import.ReactSelect
module O = FSharp.Core.Option

let [<PassGenerics>] view (): React.ReactElement =
    div [][ 
            //words 60 "Blockchain and Physical Oil"
            img [   ClassName "img-fluid"
                    Src "img/ind_er_glb_ho_2002_lo.png" ] 
            hr []
            a [ Href "http://www.avalanchain.com" ] [ words 20 "Avalanchain" ] 

    ]
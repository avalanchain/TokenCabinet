module Client.Menu

open System
open FSharp.Reflection

open Fable.Core
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Shared.Utils

open ClientMsgs
open Shared.Auth
open LoginPage
open ClientModels

module O = FSharp.Core.Option

importAll "../lib/css/dashboard.css"


let toHash =
  function
  | MenuPage.Home -> "#home"
  | MenuPage.Login -> "#login"
  | MenuPage.Cabinet tc -> "#cabinet/" + (getUnionCaseName tc typeof<CabinetPage.Page>).ToLowerInvariant()
//   | MenuPage.Trading p -> 
//     let uid = match p with 
//                 | Trading.Page.Trader ci
//                 | Trading.Page.VesselOperator ci
//                 | Trading.Page.VesselMaster ci
//                 | Trading.Page.Terminal ci
//                 | Trading.Page.Inspector ci -> "/" + ci.UID.ToString()
//                 | Trading.Page.Archive 
//                 | Trading.Page.All -> ""
//     "#trading/" + getUnionCase(p).Name.ToLowerInvariant() + uid

let init() = Utils.load<AuthModel> "user", Cmd.none

let goToUrl (e: React.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

[<PassGenerics>]
let viewLink page description icon bage isActive =
    a [ ClassName ("nav-link" + (if isActive then " active" else "")); 
        Href (toHash page) 
        OnClick goToUrl ] [
            yield i [ ClassName icon ] [ ]
            yield ofString (" " + description)  
            if bage > 0u then yield span [ ClassName "badge badge-info" ] [ ofString (bage.ToString()) ] 
    ]

[<PassGenerics>]
let navViewLink page description icon =
    li [ ClassName "nav-item" ] [ viewLink page description icon 0u false ]

let buttonLink page description icon onClick =
    a [ ClassName "nav-link"
        OnClick (fun _ -> onClick())
        OnTouchStart (fun _ -> onClick()) ] [
        i [ ClassName icon ] [ ]
        ofString (" " + description)
    ]
let navButtonLink page description icon onClick =
    li [ ClassName "nav-item" ] [ buttonLink page description icon onClick ]

[<PassGenerics>]
let cabinetNavViewLink page description icon bage isActive =
    li [ ClassName ("nav-item") ] [ viewLink page description icon bage isActive ]

let handleClick (e: React.MouseEvent) =
    Browser.console.log (sprintf "Menu  Mouse evt: '%A'" e)
    e.preventDefault()
    e.target?parentElement?classList?toggle("open") |> ignore

let view (model: AppModel) (dispatch: UIMsg -> unit) =
    let home = 
        [   navViewLink MenuPage.Home "HOME" "icon-speedometer" ]

    let login = 
        [   navViewLink MenuPage.Login "LOGIN" "icon-star" ]

    let logout = 
        [   navButtonLink MenuPage.Login "LOGOUT" "icon-star" (fun _ -> dispatch Logout) ]

    let cabinet = 
        let page (case: UnionCaseInfo) = FSharpValue.MakeUnion(case, [||]) :?> CabinetPage.Page
        
        [   li [    ClassName "nav-item nav-dropdown open" ] [
                a [ ClassName "nav-link nav-dropdown-toggle"
                    Href "#"
                    OnClick handleClick ] [ ofString " STATIC DATA" ]
                ul [ ClassName "nav-dropdown-items" ] [
                    for c in getUnionCases(typeof<CabinetPage.Page>) do 
                        yield navViewLink (c |> page |> MenuPage.Cabinet) ((c.Name |> splitOnCapital) + "s") "icon-puzzle"
                ]
            ]
        ]

    let divider = li [ ClassName "divider" ] [ ]

    div [ ClassName "sidebar" ] [
        nav [ ClassName "sidebar-nav" ] [
            ul [ ClassName "nav" ] (
                match model.Auth with
                | None ->
                        (home 
                            @ (divider :: login)
                        )
                | Some _ -> 
                        (home 
                            // @ (divider :: trading)
                            @ (divider :: cabinet)
                            @ (divider :: logout)
                            //@ (divider :: statics)
                        )
            )
        ]
    ]
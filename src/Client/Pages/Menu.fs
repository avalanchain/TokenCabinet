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

[<PassGenerics>]
let viewLink page description icon bage isActive =
    a [ ClassName ("nav-link" + (if isActive then " active" else "")); Href (toHash page) ] [
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

    // let trading = 
    //     [   li [    ClassName "nav-item nav-dropdown open" ] [
    //             a [ ClassName "nav-link nav-dropdown-toggle"
    //                 Href "#"
    //                 OnClick handleClick ] [ text " TRADING" ]
    //             ul [ ClassName "nav-dropdown-items" ] [
    //                 let icon = function
    //                             | Trading.Page.Trader ci -> "icon-refresh"
    //                             | Trading.Page.VesselOperator ci -> "icon-shuffle"
    //                             | Trading.Page.VesselMaster ci -> "icon-anchor"
    //                             | Trading.Page.Terminal ci -> "icon-direction"
    //                             | Trading.Page.Inspector ci -> "icon-eyeglass"
    //                             | Trading.Page.Archive -> "icon-docs"
    //                             | Trading.Page.All -> "icon-grid"
    //                 let mutable lastCaseName = None 
    //                 for c in Trading.pageDefs do 
    //                     let tradingPage = c |> fst
    //                     let menuPage = tradingPage |> MenuPage.Trading
    //                     let companyInfo = !!tradingPage?data
    //                     let caseName = tradingPage |> getUnionCaseNameSplit
    //                     if lastCaseName <> Some caseName then yield div [ ClassName "dropdown-divider" ] [] 
    //                     lastCaseName <- Some caseName
    //                     yield cabinetNavViewLink menuPage ( if companyInfo |> isNull then caseName else !!companyInfo?Name)
    //                         (icon tradingPage) (model.Trading |> (c |> snd) |> Map.count |> uint32) 
    //                         (match model.Page with 
    //                             | MenuPage.Trading tp -> 
    //                                 let tpci = !!tp?data
    //                                 if (companyInfo |> isNull) || (tpci |> isNull) then tp = tradingPage 
    //                                 else 
    //                                     let tpuid: uint32 = !!tpci?UID
    //                                     tpuid = !!companyInfo?UID
    //                             | _ -> false)
    //                 yield div [ ClassName "dropdown-divider" ] []
    //             ]
    //         ]
    //     ]


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
module Client.Menu

open System
open FSharp.Reflection

open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Fable.Core
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Core.JsInterop

open Elmish


open Shared.Utils

open ClientMsgs
open Shared.Auth
open LoginPage
open ClientModels

module O = FSharp.Core.Option



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
    // e.stopPropagation()
    // e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

[<PassGenerics>]
let viewLink page description icon bage =
    a [ Href (toHash page) 
        OnClick goToUrl] [
            yield i [ ClassName icon ] [ ]
            yield  span [ Class "nav-label" ]
                                [ str description ]  
            if bage > 0u then yield span [ ClassName "pull-right label label-primary" ] [ ofString (bage.ToString()) ] 
    ]

[<PassGenerics>]
let navViewLink page description icon isActive =
    li [ ClassName (if isActive then " active" else "") ] 
       [ viewLink page description icon 0u ]

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
    li [ ClassName ("nav-item" + (if isActive then " active" else "")) ] [ viewLink page description icon bage  ]

let handleClick (e: React.MouseEvent) =
    Browser.console.log (sprintf "Menu  Mouse evt: '%A'" e)
    e.preventDefault()
    e.target?parentElement?classList?toggle("open") |> ignore


// let testlink = 

let view (model: AppModel) (dispatch: UIMsg -> unit) =
    let icon  (page: CabinetPage.Page) = 
                match page with 
                | CabinetPage.Page.Verification      -> "fa fa-address-card"    
                | CabinetPage.Page.PurchaseToken     -> "fa fa-shopping-cart"  
                | CabinetPage.Page.MyInvestments     -> "fa fa-briefcase"  
                | CabinetPage.Page.ReferralProgram   -> "fa fa-refresh"
                | CabinetPage.Page.Contacts          -> "fa fa-phone"            
                | CabinetPage.Page.Dashboard         -> "fa fa-th-large"         
    let home = 
        [   navViewLink MenuPage.Home "Main view" "fa fa-th-large" ( model.Page = MenuPage.Home )]

    let login = 
        [   navViewLink MenuPage.Login "LOGIN" "fa fa-sign-in" ( model.Page = MenuPage.Login )]

    let cabinet = 
        let toPage (case: UnionCaseInfo) = FSharpValue.MakeUnion(case, [||]) :?> CabinetPage.Page
        
        [   for page in getUnionCases(typeof<CabinetPage.Page>) ->
                let pageName = page.Name |> splitOnCapital 
                let page = page |> toPage
                navViewLink (MenuPage.Cabinet page) pageName (icon page) (MenuPage.Cabinet page = model.Page)
        ]

    // let divider = li [ ClassName "divider" ] [ ]

    let staticPart = 
        li [ Class "nav-header" ]
            [   div [ Class "dropdown profile-element" ]
                                        [ a [ DataToggle "dropdown"
                                              Class "dropdown-toggle"
                                              Href "#" ]
                                            [ span [ Class "clear" ]
                                                   [ img [ Class "logo"
                                                           Src "../lib/img/avalanchain.png" ] ] ]
                                           ]
                div [ Class "logo-element" ]
                    [ ofString "TC" ] 
            ]

    let dynamicPart: React.ReactElement list = 
                match model.Auth with
                    | None ->
                            (home @ login
                            )
                    | Some _ -> 
                            (cabinet)
                            
                    

    nav [ Class "navbar-default navbar-static-side"
          Role "navigation" ]
            [ div [ Class "sidebar-collapse" ]
                [ ul [ Class "nav metismenu"
                       Id "side-menu" ]
                    
                        (staticPart :: dynamicPart)
                            ] ] 
    // div [ ClassName "sidebar" ] [
    //     nav [ ClassName "sidebar-nav" ] [
    //         ul [ ClassName "nav" ] (
    //             match model.Auth with
    //             | None ->
    //                     (home 
    //                         @ (login)
    //                     )
    //             | Some _ -> 
    //                     (home 
    //                         // @ (divider :: trading)
    //                         @ (cabinet)
    //                         @ (logout)
    //                         //@ (divider :: statics)
    //                     )
    //         )
    //     ]
    // ]
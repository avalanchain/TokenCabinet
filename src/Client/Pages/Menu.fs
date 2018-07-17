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
open LoginCommon
open Client.Page

module O = FSharp.Core.Option


let init() = Utils.load<AuthModel> "user", Cmd.none


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

let view (menuPage: CabinetPagePage) (dispatch: UIMsg -> unit) =
    let icon  (page: CabinetPagePage) = 
                match page with 
                | Verification      -> "fa fa-address-card"    
                | PurchaseToken     -> "fa fa-shopping-cart"  
                | Investments     -> "fa fa-briefcase"  
                | ReferralProgram   -> "fa fa-refresh"
                | Contacts          -> "fa fa-phone"            
                // | Dashboard         -> "fa fa-th-large"         

    let cabinet = 
        let toPage (case: UnionCaseInfo) = FSharpValue.MakeUnion(case, [||]) :?> CabinetPagePage
        
        [   for page in getUnionCases<CabinetPagePage> ->
                let pageName = page.Name |> splitOnCapital 
                let page = page |> toPage
                navViewLink (MenuPage.Cabinet page) pageName (icon page) (page = menuPage)
        ]

    // let divider = li [ ClassName "divider" ] [ ]

    let staticPart = 
        li [ Class "nav-header" ]
            [   div [ Class "dropdown profile-element" ]
                                        [ a [ DataToggle "dropdown"
                                              Class "dropdown-toggle"
                                              Href "#" ]
                                            [ span [ Class "clear" ]
                                                   [ img [ Class "logolong"
                                                           Src "../lib/img/token_cab_2.png" ] ] ]
                                           ]
                div [ Class "logo-element" ]
                    [ ofString "TC" ] 
            ]
                    

    nav [ Class "navbar-default navbar-static-side"
          Role "navigation" ]
            [ div [ Class "sidebar-collapse" ]
                [ ul [ Class "nav metismenu"
                       Id "side-menu" ]
                    
                        (staticPart :: cabinet)
                            ] ] 

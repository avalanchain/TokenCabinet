module Client.PageRouter

open FSharp.Reflection

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open ClientMsgs
open ClientModels
open Shared.Utils
open Menu

let staticPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
    allUnionCases(typeof<CabinetPage.Page>)
    |> List.map (fun ed -> map (ed |> MenuPage.Cabinet) (s "static" </> s ((getUnionCaseName ed typeof<CabinetPage.Page>).ToLowerInvariant())))


// let [<PassGenerics>]tradingPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
//     let parserWithUid caseName case getName = 
//         map (fun uid -> { Trading.CompanyInfo.UID = uid |> uint32; Trading.CompanyInfo.Name = uid |> uint32 |> getName } |> case |> MenuPage.Trading) (s "trading" </> s caseName </> i32)
//     Trading.pageDefs 
//     |> List.map (fun ed -> 
//                     let pd = ed |> fst
//                     match pd with 
//                     | Trading.Page.Trader ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Trader (fun uid -> (Examples.traders |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.VesselOperator ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.VesselOperator (fun uid -> (Examples.vesselOperators |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.VesselMaster ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.VesselMaster (fun uid -> (Examples.captains |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.Terminal ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Terminal (fun uid -> (Examples.terminals |> List.find (fun t -> t.Uid = uid)).Name)
//                     | Trading.Page.Inspector ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Inspector (fun uid -> (Examples.inspectors |> List.find (fun t -> t.Uid = uid)).Name)
                        
//                     | Trading.Page.Archive 
//                     | Trading.Page.All -> map (pd |> MenuPage.Trading) (s "trading" </> s (getUnionCase(pd).Name.ToLowerInvariant()) ))
                    


/// The URL is turned into a Result.
let pageParser : Parser<MenuPage -> _,_> =
    oneOf ([map MenuPage.Home (s "home")
            map MenuPage.Login (s "login") ] 
            @ staticPageParsers
            // @ tradingPageParsers 
            )
    
let urlUpdate (result: MenuPage option) (model: AppModel) =
    match result with
    | None ->
        Browser.console.error("Error parsing url:")
        ( model, Navigation.modifyUrl (Menu.toHash model.Page) )

    | Some (MenuPage.Login as page) ->
        let m,cmd = LoginPage.init model.Auth
        { model with Page = page; PageModel = LoginModel m }, Cmd.map LoginMsg cmd

    | Some (MenuPage.Cabinet p as page) ->
        match model.Auth with
        | Some user ->
            let m,cmd = CabinetPage.init user p
            { model with Page = page; PageModel = m |> CabinetModel }, Cmd.map (CabinetMsg) cmd
        | None ->
            model, Cmd.ofMsg (Logout |> UIMsg)

    // | Some (MenuPage.Trading p as page) ->
    //     match model.User with
    //     | Some user ->
    //         //let m,cmd = Trading.init user p
    //         //{ model with Page = page; SubModel = m |> TradingModel }, Cmd.map (TradingMsg) cmd
    //         { model with Page = page; SubModel = NoSubModel }, Cmd.none
    //     | None ->
    //         model, Cmd.ofMsg (Logout |> MenuMsg)

    | Some (MenuPage.Home as page) ->
        { model with Page = page }, []

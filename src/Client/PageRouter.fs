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

open Client.Page
open ClientMsgs
open ClientModels
open CabinetModel
open Shared.Utils
open Client.Page

let urlUpdate (result: MenuPage option) (model: AppModel) =
    match result with
    | None ->
        Browser.console.error("Error parsing url:")
        ( model, Navigation.modifyUrl (toHash model.Page) )

    | Some (MenuPage.Login as page) ->
        let m,cmd = LoginPage.init model.Auth
        { model with Page = page; PageModel = LoginModel m }, Cmd.map LoginMsg cmd

    | Some (MenuPage.Register as page) ->
        let m,cmd = RegisterPage.init model.Auth
        { model with Page = page; PageModel = RegisterModel m }, Cmd.map RegisterMsg cmd

    | Some (MenuPage.ForgotPassword as page) ->
        let m,cmd = ForgotPasswordPage.init model.Auth
        { model with Page = page; PageModel = ForgotPasswordModel m }, Cmd.map ForgotPasswordMsg cmd

    | Some (MenuPage.Cabinet p as page) ->
        match model.Auth with
        | Some user ->
            { model with Page = page; PageModel = model.CabinetModel |> CabinetModel }, Cmd.none
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
        { model with Page = page }, Cmd.none

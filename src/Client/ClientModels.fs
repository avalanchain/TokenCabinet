module ClientModels

open System
open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Shared
open Auth
open Client

open Fable
open Fable.Core
open Fable.Import.RemoteDev
open Fable.Import.Browser
open Fable.Import
open JsInterop

open Fable.Core.JsInterop
open CryptoCurrencyPrices

open Client.Page
open ClientMsgs
open LoginPage
open CabinetModel
open Client.LoginCommon

// type LoginState =
// | LoggedOut
// | LoggedIn of JWT

// Model

type PageModel =
    | NoPageModel
    | LoginFlowModel of LoginFlowPage.Model
    | CabinetModel   of CabinetModel.Model
//   | StaticModel of Statics.Model

type AppModel = {
    Page        : MenuPage
    PageModel   : PageModel

    Loading     : bool  
    // State       : LoginState

    // Counter     : Counter option

    // LoginFlowModel: LoginFlowPage.Model
    // CabinetModel  : CabinetModel.Model
}


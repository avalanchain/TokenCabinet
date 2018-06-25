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

open ClientMsgs
open LoginPage

// type LoginState =
// | LoggedOut
// | LoggedIn of JWT

// Model

[<RequireQualifiedAccess>]
type MenuPage = 
  | Home 
  | Login
  | Cabinet of CabinetPage.Page
//   | Static of Statics.Page
  with static member Default = Home


type PageModel =
  | NoPageModel
  | LoginModel   of LoginPage.Model
  | CabinetModel of CabinetPage.Model
//   | StaticModel of Statics.Model

type AppModel = {
    Auth        : AuthModel option
    Loading     : bool  
    Page        : MenuPage
    PageModel   : PageModel
    // State       : LoginState

    Counter     : Counter option

    CryptoCurrencies      : CryptoCurrencies.CryptoCurrency list
    CurrenciesCurentPrices: ViewModels.CurrencyPriceTick

    TokenSale             : ViewModels.TokenSale option

}


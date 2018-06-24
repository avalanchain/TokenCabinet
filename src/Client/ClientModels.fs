module ClientModels

open System
open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Shared
open Auth

open Fable
open Fable.Core
open Fable.Import.RemoteDev
open Fable.Import.Browser
open Fable.Import
open JsInterop

open Fable.Core.JsInterop
open CryptoCurrencyPrices

open ClientMsgs

[<RequireQualifiedAccess>]
type MenuPage = 
  | Home 
  | Admin
  | Login
//   | Static of Statics.Page
//   | Trading of Trading.Page
  with static member Default = Home

// type LoginState =
// | LoggedOut
// | LoggedIn of JWT

// Model

type SubModel =
  | NoSubModel
//   | LoginModel of LoginPage.Model
//   | StaticModel of Statics.Model


type AuthModel = {
    Token: AuthToken
    UserName: string
}

type Model = {
    Auth: AuthModel option

    Counter: Counter option

    CryptoCurrencies: CryptoCurrencies.CryptoCurrency list
    CurrenciesCurentPrices: ViewModels.CurrencyPriceTick

    TokenSale: ViewModels.TokenSale option
    MenuMediator: MenuMediator  
}


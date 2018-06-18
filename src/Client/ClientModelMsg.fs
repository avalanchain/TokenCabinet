module ClientModelMsg

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Shared

open Fulma

open Fulma.FontAwesome
open Fable
open Fable.Core
open Fable.Import.RemoteDev
open Fable.Import.Browser
open Fable.Import
open JsInterop


open Fable.Core.JsInterop
open CryptoCurrencyPrices

type MenuMediator =
    | Verification
    | PurchaseToken
    | MyInvestments
    | ReferralProgram
    | Contacts
    | Dashboard

type Model = {
    Counter: Counter option

    CryptoCurrencies: CryptoCurrencies.CryptoCurrency list
    CurrenciesCurentPrices: ViewModels.CurrencyPriceTick

    TokenSale: ViewModels.TokenSale option
    MenuMediator: MenuMediator  
}

type RemotingError =
    | CommunicationError of exn
    | ServerError of ServerError

type Msg =
    | Increment
    | Decrement
    | Init of Counter

    | ServerErrorMsg    of RemotingError
    | ErrorMsg          of Model * Msg

    | InitDb
    | InitDbCompleted               of unit

    | GetCryptoCurrenciesCompleted  of CryptoCurrencies.CryptoCurrency list
    | GetTokenSaleCompleted         of ViewModels.TokenSale
    | PriceTick                     of ViewModels.CurrencyPriceTick
    
    | Tick          of uint64
    | MenuSelected  of MenuMediator
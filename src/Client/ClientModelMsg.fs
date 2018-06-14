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

type MenuMediator =
    | Verification
    | PurchaseToken
    | MyInvestments
    | ReferralProgram
    | Contacts

type Model = {
    Counter: Counter option

    CryptoCurrencies: CryptoCurrencies.CryptoCurrency list

    TokenSale: ViewModels.TokenSale option
    MenuMediator: MenuMediator  
}

type RemotingError =
    | CommunicationError of exn
    | ServerError of ServerError

type Msg =
    | Increment
    | Decrement
    | Init of Result<Counter, RemotingError>

    | ErrorMsg of Model * Msg

    | InitDb
    | InitDbCompleted of Result<unit, exn>

    | GetCryptoCurrenciesCompleted of Result<CryptoCurrencies.CryptoCurrency list, RemotingError>
    | GetTokenSaleCompleted of Result<ViewModels.TokenSale, RemotingError>
    
    | MenuSelected of MenuMediator
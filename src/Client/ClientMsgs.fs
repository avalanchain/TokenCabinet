module ClientMsgs

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

type RemotingError =
    | CommunicationError of exn
    | ServerError of ServerError

type AppMsg =
    | AuthMsg       of AuthMsg
    | ServerMsg     of ServerMsg
    | UIMsg         of UIMsg
    | UnexpectedMsg of UnexpectedMsg
    | ErrorMsg      of string * AppMsg * string
    | OldMsg        of OldMsg

    | LoginMsg      of LoginPage.Msg
    | CabinetMsg    of CabinetPage.Msg

and AuthMsg =
    | LoggedIn      of Auth.AuthToken
    | LoggedOut
and ServerMsg =
    | GetCryptoCurrenciesCompleted  of CryptoCurrencies.CryptoCurrency list
    | GetTokenSaleCompleted         of ViewModels.TokenSale
    | PriceTick                     of ViewModels.CurrencyPriceTick
and UIMsg =
    | Tick                  of uint64
    | MenuSelected          of CabinetPage.Page
    | BrowserStorageUpdated
    | Login
    | Logout  
and UnexpectedMsg =
    | BrowserStorageFailure of Exception
    | ServerErrorMsg        of RemotingError
and OldMsg = 
    | Increment
    | Decrement
    | Init                  of Counter
    | InitDb
    | InitDbCompleted       of unit

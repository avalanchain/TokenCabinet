module ClientMsgs

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


type MenuMediator =
    | Verification
    | PurchaseToken
    | MyInvestments
    | ReferralProgram
    | Contacts
    | Dashboard

type RemotingError =
    | CommunicationError of exn
    | ServerError of ServerError

type Msg =
    | AuthMsg       of AuthMsg
    | ServerMsg     of ServerMsg
    | UIMsg         of UIMsg
    | UnexpectedMsg of UnexpectedMsg
    | ErrorMsg      of string * Msg * string
    | OldMsg        of OldMsg
and AuthMsg =
    | LoggedIn      of Auth.AuthToken
    | LoggedOut
and ServerMsg =
    | GetCryptoCurrenciesCompleted  of CryptoCurrencies.CryptoCurrency list
    | GetTokenSaleCompleted         of ViewModels.TokenSale
    | PriceTick                     of ViewModels.CurrencyPriceTick
and UIMsg =
    | Tick                  of uint64
    | MenuSelected          of MenuMediator
and UnexpectedMsg =
    | BrowserStorageFailure of Exception
    | ServerErrorMsg        of RemotingError
and OldMsg = 
    | Increment
    | Decrement
    | Init                  of Counter
    | InitDb
    | InitDbCompleted       of unit

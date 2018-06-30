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
open Client.Page

type RemotingError =
    | CommunicationError of exn
    | ServerError of ServerError

type AppMsg =
    | AuthMsg           of AuthMsg
    | UIMsg             of UIMsg
    | UnexpectedMsg     of UnexpectedMsg
    | ErrorMsg          of string * AppMsg * string
    | LoginFlowMsg      of LoginFlowPage.Msg
    | CabinetMsg        of CabinetModel.Msg

and AuthMsg =
    | LoggedIn      of Auth.AuthToken
    | LoggedOut
and UIMsg =
    | Tick                  of uint64
    | MenuSelected          of CabinetPagePage
    | BrowserStorageUpdated
    | Login
    | Logout  
and UnexpectedMsg =
    | BrowserStorageFailure of Exception
    | ServerErrorMsg        of RemotingError

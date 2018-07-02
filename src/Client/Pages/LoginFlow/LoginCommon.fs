module Client.LoginCommon
open Shared.Auth
open Fable.Core
open Fable.Core.Exceptions
open Fable.Import.React
open Fable.Helpers.React
open Fable.Helpers.React.Props

type AuthModel = {
    Token: AuthToken
}

type LoginState =
    | LoggedOut
    | LoggedIn  of AuthModel

[<Emit("null")>]
let emptyElement : ReactElement = jsNative
let hasErrors startedTyping (errors: List<_>) = startedTyping && not errors.IsEmpty 
let hasErrorsClass startedTyping (errors: List<_>) =
    if hasErrors startedTyping errors then "has-error" else ""
let hasErrorsSpan startedTyping (errors: List<_>) =
    if hasErrors startedTyping errors 
    then span [ Class "help-block" ] 
            [ ul [ ]  [ for error in errors -> li [ ] [ str error ] ] ]
    else emptyElement        

let [<Literal>] ENTER_KEY = 13.

let onEnter msg dispatch =
    function 
    | (ev:KeyboardEvent) when ev.keyCode = ENTER_KEY ->
        ev.preventDefault() 
        dispatch msg
    | _ -> ()
    |> OnKeyDown

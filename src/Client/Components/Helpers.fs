module Client.Helpers

open Fable.Helpers.React
open Fable.Helpers.React.Props
// open ClientMsgs
open Fable
open Shared.ViewModels
open Fable.DateFunctions
open System
open Fable.Import.React
open Fable.Core.JsInterop
open ReactBootstrap

module React =
    type Component<'P> = Fable.Import.React.Component<'P, obj>

let inline com<'P> (com: React.Component<'P>) (props: 'P ) (children: ReactElement seq): ReactElement =
    createElement(com, props, children)
let inline comF<'P> (com: React.Component<'P>) (propsFunc: 'P -> unit) (children: ReactElement seq): ReactElement =
    createElement(com, jsOptions<'P> propsFunc, children)
let inline comE<'P> (com: React.Component<'P>) (children: ReactElement seq): ReactElement =
    createElement(com, createEmpty<'P>, children)



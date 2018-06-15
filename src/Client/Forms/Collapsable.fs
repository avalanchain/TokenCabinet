namespace Client.Forms

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Client.Style
open Client.Utils

open ServerCode.Utils
open ServerCode.Commodities

open ReactStrap
open ReactJsonSchemaForm
open ReactDatePicker
open Fable.Import.Moment
open Fable.Helpers.Moment

module RSel = Fable.Import.ReactSelect
module O = FSharp.Core.Option

module Collapsable = 
    type [<Pojo>] CollapsableState<'S> = { isOpen: bool; formData: 'S }

    [<AbstractClass>]
    type Collapsable<[<Pojo>]'P, 'S>(props, initialState: unit -> 'S, btnText: string, btnClass: string) =
        inherit React.Component<'P, CollapsableState<'S>>(props)
        do base.setInitState({ isOpen = false; formData = initialState() })

        member __.Reset() = base.setInitState({ isOpen = false; formData = initialState() })
        member __.Show() = __.setState({ __.state with isOpen = true })
        member __.Hide() = __.setState({ __.state with isOpen = false })
        member __.Toggle() = __.setState({ __.state with isOpen = not __.state.isOpen })
        member __.Data with get() = __.state.formData and set v = __.setState { __.state with formData = v }

        abstract member RenderForm: React.ReactElement -> React.ReactElement
        abstract member RenderBody: unit -> React.ReactElement

        member __.render () =
            div [] [
                __.RenderForm(  button [ClassName ("btn btn-sm " + btnClass) 
                                        OnClick (fun _ -> __.Toggle())
                                        ] [ text btnText ]) 
                Collapse (fun p -> p.isOpen <- __.state.isOpen) [
                    div [ClassName "container"] [
                        div [ClassName "row"] [
                            div [ClassName "col-12"] [ __.RenderBody() ]
                        ] 
                    ] 
                ]
            ]
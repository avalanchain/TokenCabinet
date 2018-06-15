namespace Client.Forms

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types

open Elmish
open Elmish.React
open Elmish.React.Common

open ServerCode
open ServerCode.Domain
open ServerCode.Commodities
open ServerCode.Utils

open Client.Style

open ReactStrap

type [<Pojo>] ModalState<'S> = { isOpen: bool; formData: 'S }

[<AbstractClass>]
type ModalForm<[<Pojo>]'P, 'S>(props, initialState: unit -> 'S, btnText: string, btnClass: string) =
    inherit React.Component<'P, ModalState<'S>>(props)
    do base.setInitState({ isOpen = false; formData = initialState() })

    member __.Reset() = base.setInitState({ isOpen = false; formData = initialState() })
    member __.Show() = __.setState({ __.state with isOpen = true })
    member __.Hide() = __.setState({ __.state with isOpen = false })
    member __.Toggle() = __.setState({ __.state with isOpen = not __.state.isOpen })
    member __.Data with get() = __.state.formData and set v = __.setState { __.state with formData = v }

    abstract member RenderForm: unit -> React.ReactElement list
    abstract member Title: string
    abstract member OnSave: unit -> unit

    member __.render () =
        div [] [
            button [ClassName ("btn btn-sm " + btnClass) 
                    OnClick (fun _ -> __.Show())
                    ] [ text btnText ]
            Modal (fun p -> p.isOpen <- __.state.isOpen
                            p.size <- Some "large"
                            p.toggle <- fun _ -> __.Toggle()
                            p.className <- Some "modal-lg"
                            ) [
                ModalHeader (fun p -> p.toggle <- fun _ -> __.Toggle()) [ text __.Title ]
                ModalBody (ignore) [
                    div [ClassName "container"] [
                        div [ClassName "row"] [
                            div [ClassName "col-12"] (__.RenderForm())
                        ] 
                    ] 
                ]

                ModalFooter (ignore) [
                    button [ClassName "btn btn-primary" 
                            OnClick (fun _ ->   __.OnSave() 
                                                __.Reset())
                            ] [ text "Save" ]
                    button [ClassName "btn btn-secondary" 
                            OnClick (fun _ ->   __.Hide() 
                                                __.Reset())
                            ] [ text "Cancel" ]
                ]
            ]
        ]
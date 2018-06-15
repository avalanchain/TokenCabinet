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

open FormHelpers

module O = FSharp.Core.Option

type [<Pojo>] VesselProgressProps = { 
    //Vessel: Vessel
    DepartureInfo: CaptainSigned<TerminalSigned<VesselDepartureInfo>>
    Progress: CaptainSigned<DeliveryProgress> option
    Signer: DeliveryProgress -> CaptainSigned<DeliveryProgress>
    Dispatch: CaptainSigned<DeliveryProgress> -> unit
}

type [<Pojo>] VesselProgressState = { progress: float<percent> }

type VesselProgressForm(props) =
    inherit ModalForm<VesselProgressProps, VesselProgressState>(
        props, (fun () -> { progress = match props.Progress with Some dp -> dp.V.Progress | None -> 0.<percent> }), "Notify of Vessel progress", "btn-success")

    let toProgressStatus (value: float<percent>) = toNumberStatus (value |> float)
    let isEmptyProgressError fieldCaption (value: float<percent>) = isEmptyNumberError fieldCaption (value |> float)

    member __.Update p = __.Data <- { progress = p }

    override __.RenderForm () = [ numberField 1. {  value = __.Data.progress
                                                    onChange = fun reason -> __.Update reason
                                                    fieldName = "Progress"
                                                    required = true 
                                                    title = None
                                                    iconClass = "glyphicon-pencil"
                                                    placeholder = Some "Progress"
                                                    fieldStatus = toProgressStatus
                                                    validators = isEmptyProgressError } ]

    override __.OnSave () = __.props.Dispatch ({ Progress = __.Data.progress } |> __.props.Signer) 
    override __.Title = "Please indicate Vessel's progress"
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

type [<Pojo>] RejectionModalProps = { 
    Deal: DealToSchedule
    Signer: DealToSchedule * RejectionReason -> BuyerSigned<DealToSchedule * RejectionReason>
    Dispatch: BuyerSigned<DealToSchedule * RejectionReason> -> unit
}

type [<Pojo>] RejectionModalState = { RejectionReason: RejectionReason }

type RejectionForm(props) =
    inherit ModalForm<RejectionModalProps, RejectionModalState>(props, (fun () -> { RejectionReason = { Reason = "" } }), "Reject", "btn-danger")

    member __.Update reason = __.Data <- { RejectionReason = { Reason = reason } }

    override __.RenderForm () = [ textField {   value = __.Data.RejectionReason.Reason
                                                onChange = fun reason -> __.Update reason
                                                fieldName = "Rejection reason"
                                                required = true 
                                                title = None
                                                iconClass = "glyphicon-pencil"
                                                placeholder = None
                                                fieldStatus = toStatus
                                                validators = isEmptyError } ]

    override __.OnSave () = __.props.Dispatch ((__.props.Deal, __.state.formData.RejectionReason) |> __.props.Signer) 
    override __.Title = "Reason for Rejection"
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

type [<Pojo>] RejectionNominationModalProps = { 
    VesselNomination: VesselNomination
    Signer: VesselNomination * RejectionReason -> BuyerSigned<VesselNomination * RejectionReason>
    Dispatch: BuyerSigned<VesselNomination * RejectionReason> -> unit
}

type [<Pojo>] RejectionNominationModalState = { RejectionReason: RejectionReason }

type RejectNominationForm(props) =
    inherit ModalForm<RejectionNominationModalProps, RejectionNominationModalState>(props, (fun () -> { RejectionReason = { Reason = "" } }), "Reject Nomination", "btn-danger")

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

    override __.OnSave () = __.props.Dispatch ((__.props.VesselNomination, __.state.formData.RejectionReason) |> __.props.Signer) 
    override __.Title = "Reason for rejecting Nomination"
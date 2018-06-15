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

open Client.Forms.ReadonlyViews

type [<Pojo>] VesselNominateFormProps = { 
    ButtonCaption: string
    Offers: VesselOperatorSigned<VesselOffer> list
    Signer: VesselOperatorSigned<VesselOffer> -> NominatedVessel
    Dispatch: NominatedVessel -> unit
}

type [<Pojo>] VesselNominateFormState = { VesselOffer: VesselOperatorSigned<VesselOffer> option }

type VesselNominateForm(props) =
    inherit ModalForm<VesselNominateFormProps, VesselNominateFormState>(
        props, (fun () -> { VesselOffer = None }), props.ButtonCaption, "btn-success")

    member __.Update state = __.Data <- state

    override __.RenderForm () = 
        (   __.props.Offers
            |> List.mapi (fun i vesselOffer -> 
                            div [ClassName "radio"] [
                                let id = "radio" + i.ToString()
                                yield label [HtmlFor id] [
                                    input [ Type "radio" 
                                            Id id
                                            Name "nomRadios"
                                            Value (U2.Case1 (i.ToString()))
                                            OnChange (fun (ev: React.FormEvent) -> 
                                                        __.Update { __.Data with VesselOffer = __.props.Offers.[(!!ev.target?value |> Int32.Parse)] |> Some })
                                        ] 
                                    vesselOfferView vesselOffer
                                ]
                            ])
         )

    override __.OnSave () = if __.Data.VesselOffer.IsSome  
                            then __.Data.VesselOffer.Value |> __.props.Signer |> __.props.Dispatch
    override __.Title = "Please nominate a Vessel"
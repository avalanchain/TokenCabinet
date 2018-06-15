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

type [<Pojo>] VesselOfferFormProps = { 
    VesselRequest: SellerSigned<VesselRequest>
    Fleet: Vessel list
    Signer: VesselOffer -> VesselOperatorSigned<VesselOffer>
    Dispatch: VesselOperatorSigned<VesselOffer> -> unit
}

type [<Pojo>] VesselOfferFormState = {  Vessel: Vessel option
                                        Price: Price 
                                        AvailabilityDate: DateTime }

type VesselOfferForm(props) =
    inherit ModalForm<VesselOfferFormProps, VesselOfferFormState>(
        props, (fun () -> { Vessel = None; Price = 0.<usd>; AvailabilityDate = props.VesselRequest.V.DepartureDate }), "Offer a Vessel", "btn-success")

    member __.Update state = __.Data <- state

    override __.RenderForm () = [   
            div [ClassName "row"] [
                div [ClassName "col-6"] [
                    datePickerField {   value = __.Data.AvailabilityDate
                                        onChange = fun v -> __.Update { __.Data with AvailabilityDate = v }
                                        fieldName = "LoadDate"
                                        required = true 
                                        title = None
                                        iconClass = "glyphicon-pencil"
                                        placeholder = None
                                        fieldStatus = fun _ -> "has-success" //toStatus
                                        validators = fun _ _ -> [] }
                ]
                div [ClassName "col-6"] [
                    numberField 100. {  value = __.Data.Price 
                                        onChange = fun v -> __.Update { __.Data with Price = v }
                                        fieldName = "Price"
                                        required = true 
                                        title = None
                                        iconClass = "glyphicon-pencil"
                                        placeholder = None
                                        fieldStatus = toPriceStatus
                                        validators = isEmptyPriceError } ]
            ]
            div [ClassName "row"] [
                div [ClassName "col-12"] 
                    (   __.props.Fleet
                        |> List.mapi (fun i vessel -> 
                                        div [ClassName "radio"] [
                                            let id = "radio" + i.ToString()
                                            yield label [HtmlFor id] [
                                                input [ Type "radio" 
                                                        Id id
                                                        Name "fleetRadios"
                                                        Value (U2.Case1 (i.ToString()))
                                                        OnChange (fun (ev: React.FormEvent) -> 
                                                                    __.Update { __.Data with Vessel = __.props.Fleet.[(!!ev.target?value |> Int32.Parse)] |> Some })
                                                    ] 
                                                text (" " + vessel.ToString() )
                                            ]
                                        ]))
            ]
    ]

    override __.OnSave () = if __.Data.Vessel.IsSome && __.Data.Price > 0.<usd> 
                            then {  VesselOffer.Vessel = __.Data.Vessel.Value
                                    Price = __.Data.Price 
                                    AvailabilityDate = __.Data.AvailabilityDate } |> __.props.Signer |> __.props.Dispatch
    override __.Title = "Please select a Vessel"
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

type [<Pojo>] InspectionFormProps = { 
    DepartureInfo: CaptainSigned<TerminalSigned<VesselDepartureInfo>>
    ArrivalInfo: CaptainSigned<ArrivalInfo>
    Signer: InspectionResult -> InspectorSigned<InspectionResult>
    Dispatch: InspectorSigned<InspectionResult> -> unit
}

type [<Pojo>] InspectionFormState = { InspectionResult: InspectionResult }

type InspectionForm(props) =
    inherit ModalForm<InspectionFormProps, InspectionFormState>(
        props, (fun () ->
                    let contractTerms = props.DepartureInfo.V.V.Nomination.CounterSignedDeal.V.V 
                    { InspectionResult = {  ActualDeliveryDate = props.ArrivalInfo.V.Date
                                            QualitySpecs = contractTerms.QualitySpecs
                                            Quantity = contractTerms.Quantity } }), "Perform Inspection", "btn-success")

    member __.Update result = __.Data <- { __.Data with InspectionResult = result }

    override __.RenderForm () = [   
            div [ClassName "row"] [
                div [ClassName "col-6"] [
                    datePickerField {   value = __.Data.InspectionResult.ActualDeliveryDate
                                        onChange = fun v -> __.Update { __.Data.InspectionResult with ActualDeliveryDate = v }
                                        fieldName = "ActualDeliveryDate"
                                        required = true 
                                        title = None
                                        iconClass = "glyphicon-pencil"
                                        placeholder = None
                                        fieldStatus = fun _ -> "has-success" //toStatus
                                        validators = fun _ _ -> [] }
                ]
            ]
            div [ClassName "row"] [
                div [ClassName "col-12"] [
                    qualitySpecsField { value = __.Data.InspectionResult.QualitySpecs
                                        onChange = fun v -> __.Update { __.Data.InspectionResult with QualitySpecs = v }
                                        fieldName = "QualitySpecs"
                                        required = true 
                                        title = None
                                        iconClass = "glyphicon-pencil"
                                        placeholder = None
                                        fieldStatus = fun _ -> "" //toStatus
                                        validators = fun _ _ -> [] }
                ]
            ]
            div [ClassName "row"] [
                div [ClassName "col-12"] [
                    volumeField {   value = __.Data.InspectionResult.Quantity
                                    onChange = fun v -> __.Update { __.Data.InspectionResult with Quantity = v }
                                    fieldName = "Quantity"
                                    required = true 
                                    title = None
                                    iconClass = "glyphicon-pencil"
                                    placeholder = None
                                    fieldStatus = fun _ -> "" //toStatus
                                    validators = fun _ _ -> [] }
                ]
            ]
    ]

    override __.OnSave () = __.Data.InspectionResult |> __.props.Signer |> __.props.Dispatch
    override __.Title = "Please confirm that delivered parameters are correct"
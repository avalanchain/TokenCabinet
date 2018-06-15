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

type [<Pojo>] TerminalDocumentsFormProps = { 
    Nomination: VesselNomination
    BillOfLading: BillOfLading option //TerminalSigned<BillOfLading> option
    Dispatch: BillOfLading -> unit
}

type [<Pojo>] TerminalDocumentsFormState = { BoLInfo: BoLInfo }

type TerminalDocumentsForm(props) =
    inherit ModalForm<TerminalDocumentsFormProps, TerminalDocumentsFormState>(
        props, (fun () ->
                    let contractTerms = props.Nomination.CounterSignedDeal.V.V 
                    { BoLInfo = match props.BillOfLading with 
                                | Some bols -> 
                                    let bol = bols //|> fst
                                    {   CargoReferenceNumber = bol.CargoReferenceNumber
                                        BoLNumber = bol.BoLNumber
                                        Quantity = bol.Quantity
                                        SupportingDocuments = bol.SupportingDocuments }
                                | None -> { CargoReferenceNumber = ""
                                            BoLNumber = ""
                                            Quantity = contractTerms.Quantity
                                            SupportingDocuments = [] } }), "Release Shipping Documents", "btn-success")

    member __.Update bolInfo = __.Data <- { __.Data with BoLInfo = bolInfo }

    override __.RenderForm () = 
        [   textField { value = __.Data.BoLInfo.CargoReferenceNumber
                        onChange = fun v -> __.Update { __.Data.BoLInfo with CargoReferenceNumber = v }
                        fieldName = "CargoReferenceNumber"
                        required = true 
                        title = None
                        iconClass = "glyphicon-pencil"
                        placeholder = None
                        fieldStatus = toStatus
                        validators = isEmptyError }
            textField { value = __.Data.BoLInfo.BoLNumber
                        onChange = fun v -> __.Update { __.Data.BoLInfo with BoLNumber = v }
                        fieldName = "BoLNumber"
                        required = true 
                        title = None
                        iconClass = "glyphicon-pencil"
                        placeholder = None
                        fieldStatus = toStatus
                        validators = isEmptyError }
            volumeField {   value = __.Data.BoLInfo.Quantity
                            onChange = fun v -> __.Update { __.Data.BoLInfo with Quantity = v }
                            fieldName = "Quantity"
                            required = true 
                            title = None
                            iconClass = "glyphicon-pencil"
                            placeholder = None
                            fieldStatus = fun _ -> "" //toStatus
                            validators = fun _ _ -> [] }
            fileUploadField  {  value = __.Data.BoLInfo.SupportingDocuments
                                onChange = fun v -> __.Update { __.Data.BoLInfo with SupportingDocuments = v }
                                fieldName = "Supporting Documents"
                                required = true 
                                title = None
                                iconClass = "glyphicon-pencil"
                                placeholder = None
                                fieldStatus = fun _ -> "" //toStatus
                                validators = fun _ _ -> [] }
    ]

    override __.OnSave () = 
        let bolInfo = __.Data.BoLInfo 
        {   CargoReferenceNumber = bolInfo.CargoReferenceNumber
            Seller = __.props.Nomination.CounterSignedDeal.V.V.Seller
            Buyer = __.props.Nomination.CounterSignedDeal.V.V.Buyer
            Carrier = __.props.Nomination.NominatedVessel.V.V |> fun v -> v.Vessel
            BoLNumber = bolInfo.BoLNumber
            Quantity = bolInfo.Quantity
            SupportingDocuments = bolInfo.SupportingDocuments }  
        |> __.props.Dispatch
    override __.Title = "Please enter shipping information"
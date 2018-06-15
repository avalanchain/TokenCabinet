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

type [<Pojo>] CreateNewContractModalProps = { 
    Title: string
    ContractTerms: ContractTerms
    Buyers: Buyer[]
    Inspectors: Inspector[]
    LoadTerminals: Terminal[]
    DischargeTerminals: Terminal[]
    Signer: ContractTerms -> DealToSchedule
    Dispatch: DealToSchedule -> unit
}

type [<Pojo>] CreateNewContractModalState = { Deal: ContractTerms }

type CreateNewContractForm(props) =
    inherit ModalForm<CreateNewContractModalProps, CreateNewContractModalState>(props, (fun () -> { Deal = props.ContractTerms }), props.Title, "btn-success")

    member __.Update deal = __.Data <- { Deal = deal }

    override __.OnSave () = __.props.Dispatch (__.Data.Deal |> __.props.Signer) 
    override __.Title = __.props.Title 
    override __.RenderForm () = 
        [   enumField<ContractType> {   value = __.state.formData.Deal.ContractType
                                        onChange = fun v -> __.Update { __.Data.Deal with ContractType = v }
                                        fieldName = "ContractType"
                                        required = true 
                                        title = None
                                        iconClass = "glyphicon-pencil"
                                        placeholder = None
                                        fieldStatus = fun _ -> "has-success" //toStatus
                                        validators = fun _ _ -> [] }

            organizationField 
                props.Buyers
                {   value = __.state.formData.Deal.Buyer
                    onChange = fun v -> __.Update { __.Data.Deal with Buyer = v }
                    fieldName = "Buyer"
                    required = true 
                    title = None
                    iconClass = "glyphicon-pencil"
                    placeholder = None
                    fieldStatus = fun _ -> "has-success" //toStatus
                    validators = fun _ _ -> [] }

            organizationField 
                props.Inspectors
                {   value = __.Data.Deal.Inspector
                    onChange = fun v -> __.Update { __.Data.Deal with Inspector = v }
                    fieldName = "Inspector"
                    required = true 
                    title = None
                    iconClass = "glyphicon-pencil"
                    placeholder = None
                    fieldStatus = fun _ -> "has-success" //toStatus
                    validators = fun _ _ -> [] }

            numberField 100. {  value = __.Data.Deal.Price 
                                onChange = fun v -> __.Update { __.Data.Deal with Price = v }
                                fieldName = "Price"
                                required = true 
                                title = None
                                iconClass = "glyphicon-pencil"
                                placeholder = None
                                fieldStatus = toPriceStatus
                                validators = isEmptyPriceError }

            qualitySpecsField { value = __.Data.Deal.QualitySpecs
                                onChange = fun v -> __.Update { __.Data.Deal with QualitySpecs = v }
                                fieldName = "QualitySpecs"
                                required = true 
                                title = None
                                iconClass = "glyphicon-pencil"
                                placeholder = None
                                fieldStatus = fun _ -> "" //toStatus
                                validators = fun _ _ -> [] }

            volumeField {   value = __.Data.Deal.Quantity
                            onChange = fun v -> __.Update { __.Data.Deal with Quantity = v }
                            fieldName = "Quantity"
                            required = true 
                            title = None
                            iconClass = "glyphicon-pencil"
                            placeholder = None
                            fieldStatus = fun _ -> "" //toStatus
                            validators = fun _ _ -> [] }

            div [ClassName "row"] [
                div [ClassName "col-8"] [
                    organizationField 
                        props.LoadTerminals
                        {   value = __.Data.Deal.TerminalLoad
                            onChange = fun v -> __.Update { __.Data.Deal with TerminalLoad = v }
                            fieldName = "LoadTerminal"
                            required = true 
                            title = None
                            iconClass = "glyphicon-pencil"
                            placeholder = None
                            fieldStatus = fun _ -> "has-success" //toStatus
                            validators = fun _ _ -> [] } ]
                div [ClassName "col-4"] [
                    datePickerField {   value = __.Data.Deal.DateLoad
                                        onChange = fun v -> __.Update { __.Data.Deal with DateLoad = v }
                                        fieldName = "LoadDate"
                                        required = true 
                                        title = None
                                        iconClass = "glyphicon-pencil"
                                        placeholder = None
                                        fieldStatus = fun _ -> "has-success" //toStatus
                                        validators = fun _ _ -> [] }
                ]
            ]

            div [ClassName "row"] [
                div [ClassName "col-8"] [
                    organizationField 
                        props.DischargeTerminals
                        {   value = __.Data.Deal.TerminalDischarge
                            onChange = fun v -> __.Update { __.Data.Deal with TerminalDischarge = v }
                            fieldName = "LoadDischarge"
                            required = true 
                            title = None
                            iconClass = "glyphicon-pencil"
                            placeholder = None
                            fieldStatus = fun _ -> "has-success" //toStatus
                            validators = fun _ _ -> [] }  ]
                div [ClassName "col-4"] [
                    datePickerField {   value = __.Data.Deal.DateDischarge
                                        onChange = fun v -> __.Update { __.Data.Deal with DateDischarge = v }
                                        fieldName = "DischargeDate"
                                        required = true 
                                        title = None
                                        iconClass = "glyphicon-pencil"
                                        placeholder = None
                                        fieldStatus = fun _ -> "has-success" //toStatus
                                        validators = fun _ _ -> [] }
                ]
            ]
        ]

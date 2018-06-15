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

open Collapsable
open ReadonlyViews

module CustomControls =
    type [<Pojo>] EnumControlProps = {  formData: obj 
                                        cases: obj[]
                                        caseNames: string[]
                                        onChange: obj -> unit }
    type EnumControl(props: EnumControlProps) =
        inherit React.Component<EnumControlProps, obj>(props)

        [<PassGenerics>]
        member __.render () =
            RSel.ReactSelect(fun p ->   p.value <- __.props.cases |> Array.tryFindIndex (fun c -> c = __.props.formData) |> O.map (fun i -> i |> float :> obj)
                                        p.onChange <- Func<_,_>(function
                                                                | U3.Case1 (opt: RSel.Option) -> 
                                                                    match opt.value with 
                                                                    | Some (U3.Case2 fi) -> __.props.onChange(__.props.cases.[fi |> int])
                                                                    | _ -> ()
                                                                | _ -> ()
                                                            ) |> Some 
                                        p.options <- __.props.cases 
                                                        |> Array.mapi (fun i c -> RSel.Option (fun o -> o.value <- i |> float |> U3.Case2 |> Some
                                                                                                        o.label <- __.props.caseNames.[i] |> Some ))
                                                        |> Some ) []

    let [<PassGenerics>] enumControl<'T> (value: 'T) onChange =
        com<EnumControl, _, _>({    formData = value
                                    onChange = fun v -> onChange (v :?> 'T) 
                                    cases = allUnionCases<'T>() |> List.map (fun c -> c :> obj) |> List.toArray
                                    caseNames = getUnionCaseNames<'T> |> List.map (splitOnCapital) |> List.toArray}) []

    type VolumeControl(props: EnumControlProps) =
        inherit React.Component<EnumControlProps, obj>(props)
        let value: Volume = !!props.formData

        member __.OnChange (volume: Volume) = volume |> box |> props.onChange 

        [<PassGenerics>]
        member __.render () =
            div [ ClassName "row" ] [
                div [ClassName "col-8"] [ 
                    input [ ClassName "form-control"
                            Type "number"
                            Value (!!value?data)
                            Step (U2.Case1 100.) 
                            OnChange(fun (ev:React.FormEvent) -> 
                                let newValue = value 
                                newValue?data <- ev.target?value
                                newValue |> __.OnChange) 
                            ] 
                ]
                div [ClassName "col-4"] [
                    RSel.ReactSelect(fun p ->   p.value <- !!value?tag |> Some
                                                p.onChange <- Func<_,_>(function
                                                                        | U3.Case1 (opt: RSel.Option) -> 
                                                                            match opt.value with 
                                                                            | Some (U3.Case2 fi) -> 
                                                                                let newValue = value 
                                                                                newValue?tag <- fi 
                                                                                newValue |> __.OnChange
                                                                            | _ -> ()
                                                                        | _ -> ()
                                                                    ) |> Some 
                                                p.options <- __.props.cases 
                                                                |> Array.mapi (fun i c -> RSel.Option (fun o -> o.value <- i |> float |> U3.Case2 |> Some
                                                                                                                o.label <- __.props.caseNames.[i] |> Some ))
                                                                |> Some ) []
                ]
            ]

    let [<PassGenerics>] volumeControl (value: Volume) onChange =
        com<VolumeControl, _, _>({  formData = value
                                    onChange = unbox >> onChange
                                    cases = allUnionCases<Volume>() |> List.map (fun c -> c :> obj) |> List.toArray
                                    caseNames = getUnionCaseNames<Volume> |> List.map (splitOnCapital) |> List.toArray}) []

    type [<Pojo>] OrganizationControlProps = {  formData: obj 
                                                onChange: obj -> unit
                                                organizations: Organization[] }
    type OrganizationControl(props: OrganizationControlProps) =
        inherit React.Component<OrganizationControlProps, obj>(props)

        [<PassGenerics>]
        member __.render () =
            div [] [
                RSel.ReactSelect(fun p ->   p.value <- __.props.organizations |> Array.tryFindIndex (fun c -> c = unbox __.props.formData) |> O.map (fun i -> i |> float :> obj)
                                            p.onChange <- Func<_,_>(function
                                                                | U3.Case1 (opt: RSel.Option) -> 
                                                                    match opt.value with 
                                                                    | Some (U3.Case2 fi) -> __.props.onChange(__.props.organizations.[fi |> int] |> box)
                                                                    | _ -> ()
                                                                | _ -> ()
                                                            ) |> Some 
                                            p.options <- props.organizations 
                                                            |> Array.mapi (fun i c -> RSel.Option (fun o -> o.value <- i |> float |> U3.Case2 |> Some
                                                                                                            o.label <- (match props.organizations.[i] with 
                                                                                                                        | Ltd p -> p.LegalEntity.Name, p.Representative.FirstName, p.Representative.MiddleName, p.Representative.LastName 
                                                                                                                        | Corporate c -> c.Name, c.Headquaters.Representative.FirstName, c.Headquaters.Representative.MiddleName, c.Headquaters.Representative.LastName ) 
                                                                                                                        |> fun (cn, fn, mn, ln) -> sprintf "%s, Contact: %s %s %s" cn fn mn ln
                                                                                                                        |> Some ))
                                                            |> Some ) []
            ]

    let organizationControl (value: Organization) onChange organizations = 
        com<OrganizationControl, _, _>({formData = value 
                                        onChange = unbox >> onChange 
                                        organizations = organizations }) []

    type QualitySpecsControl(props: EnumControlProps) =
        inherit React.Component<EnumControlProps, obj>(props)

        let onChange: QualitySpecs -> unit = box >> props.onChange
        let value: QualitySpecs = !!props.formData

        [<PassGenerics>]
        member __.render () =
            div [ClassName "row"] [
                div [ ClassName "col-6" ] [
                    div [ClassName "row"] [ 
                        label [ClassName "col-3 col-form-label"] [ text "Density" ]
                        div [ClassName "col-9"] [
                            input [ ClassName "form-control"
                                    Type "number"
                                    Value (!!__.props.formData?Density)
                                    Step (U2.Case1 0.05) 
                                    OnChange (fun (ev:React.FormEvent) -> 
                                        { value with Density = (!!ev.target?value) } |> onChange)
                                    ] 
                        ]
                    ]
                ]
                
                div [ ClassName "col-6" ] [
                    div [ClassName "form-group row"] [ 
                        label [ClassName "col-4 col-form-label"] [ text "Sulphur Level" ]
                        div [ClassName "col-8"] [
                            input [ ClassName "form-control"
                                    Type "number"
                                    Value (!!__.props.formData?SulphurLevel)
                                    Step (U2.Case1 0.0001) 
                                    OnChange (fun (ev:React.FormEvent) -> 
                                        { value with SulphurLevel = (!!ev.target?value) } |> onChange)
                                    ] 
                        ]
                    ]
                ]
            ]

    let qualitySpecsControl (value: QualitySpecs) onChange = 
        com<QualitySpecsControl, _, _>([ "formData" ==> value; "onChange" ==> onChange ] |> createObj |> unbox) []

    type [<Pojo>] DatePickerControlProps = { value: DateTime; onChange: DateTime -> unit }
    type [<Pojo>] DatePickerControlState = { value: DateTime }
    type DatePickerControl(props: DatePickerControlProps) =
        inherit React.Component<DatePickerControlProps, DatePickerControlState>(props)
        do base.setInitState({ value = props.value })

        [<PassGenerics>]
        member __.render () =
            div [ClassName "form-group"] [
                ReactDatePicker (fun p ->   p.dateFormat <- "DD/MM/YYYY" |> U2.Case1 |> Some
                                            p.selected <- __.state.value |> moment.Invoke |> U2.Case1 |> Some
                                            p?onChange <- fun (m: Moment) ->
                                                            let newValue = m.toDate()
                                                            __.setState({ __.state with value = newValue })
                                                            newValue |> __.props.onChange
                    ) 
            ]

    let datePickerControl (value: DateTime) onChange = com<DatePickerControl, _, _>({ value = value; onChange = onChange }) []

    
    type [<Pojo>] FileUploadControlProps = { value: SupportingDocuments; onChange: SupportingDocuments -> unit }
    type FileUploadControl(props: FileUploadControlProps) =
        inherit React.Component<FileUploadControlProps, obj>(props)

        [<PassGenerics>]
        member __.render () =
            div [] [
                div [ClassName "form-group"] [
                    input [ Type "file" 
                            Multiple true 
                            OnChange (fun (ev:React.FormEvent) -> 
                                        Browser.console.log(ev)
                                        ev.preventDefault()
                                        let (files: FileList) = !!ev.target?files 
                                        let mutable filesCount = files.length |> int
                                        let ret = Array.init<SupportingDocument> filesCount (fun i -> { Name = ""; Data = "" })
                                        for i in 0 .. filesCount - 1 do
                                            let f = files.item (float i) 
                                            let reader = Browser.FileReader.Create()
                                            reader.onloadend <- 
                                                fun e ->    
                                                    let i0 = i // capturing indexer
                                                    filesCount <- filesCount - 1
                                                    ret.[i0] <- {   Name = f.name
                                                                    Data = reader.result.ToString() }
                                                    if filesCount <= 0 then __.props.onChange (ret |> Array.toList)
                                                    null
                                            reader.readAsDataURL(f)
                   )
                            ]
                ]
                supportingDocumentsView __.props.value
            ]

    let fileUploadControl (value: SupportingDocuments) onChange = com<FileUploadControl, _, _>({ value = value; onChange = onChange }) []


//TODO: Move this func out of here
    let contractTerms = 
        function
        | Created state -> state.Deal.V
        | SentForAgreement state -> state.Deal.V
        | BuyerRejected state -> (state.RejectedDeal.V |> fst).V
        | BuyerAccepted state -> state.CounterSignedDeal.V.V
        | VesselRequested state -> state.CounterSignedDeal.V.V
        | VesselOffered state -> state.CounterSignedDeal.V.V
        | VesselNominated state -> state.Nomination.CounterSignedDeal.V.V
        | VesselNominationAccepted state -> state.AcceptedNomination.V.CounterSignedDeal.V.V
        | VesselNominationRejected state -> (state.RejectedNomination.V |> fst).CounterSignedDeal.V.V
        | NORReleased state -> state.Nomination.CounterSignedDeal.V.V
        | LoadBoLCreated state -> state.DepartureInfo.V.Nomination.CounterSignedDeal.V.V
        | InTransitOrUnpaid state -> state.DepartureInfo.V.V.Nomination.CounterSignedDeal.V.V
        | DeliveredAndPaid (di, ds, ps) -> di.V.V.Nomination.CounterSignedDeal.V.V


    type [<Pojo>] ContractStateControlProps = { state: SellerContractState }
    type ContractStateControl(props) =
        inherit Collapsable<ContractStateControlProps, SellerContractState>(props, (fun _ -> props.state), props.state.StateName, "btn-link")

        let valueDt: DateTime = (!!props?formData) 
        let value = valueDt |> moment.Invoke
        let onChange: DateTime -> unit = !!props?onChange

        [<PassGenerics>]
        override __.RenderForm collapseButton = 
            let ct = __.props.state |> contractTerms
            div [] [
                // contractTermsView ct
                // hr []
                dl [ ClassName "row"] [
                    dd [ ClassName "col-6" ] [
                        dl [ ClassName "row"] [
                            dt [ ClassName "col-3" ] [ text "Status: " ]
                            dd [ ClassName "col-9" ] [ collapseButton ]
                        ]
                    ]
                    dd [ ClassName "col-6" ] [
                        dl [ ClassName "row"] [
                            dt [ ClassName "col-3" ] [ text "Product: " ]
                            dd [ ClassName "col-9" ] [ text "Diesel" ]
                        ]
                    ]

                    dd [ ClassName "col-6" ] [
                        dl [ ClassName "row"] [
                            dt [ ClassName "col-3" ] [ text "Seller: " ]
                            dd [ ClassName "col-9" ] [ text (ct.Seller.Name) ]
                            dd [ ClassName "col-9 offset-3" ] [ text (ct.Seller.Representative.Name) ]
                        ]
                    ]
                    dd [ ClassName "col-6" ] [
                        dl [ ClassName "row"] [
                            dt [ ClassName "col-3" ] [ text "Buyer: " ]
                            dd [ ClassName "col-9" ] [ text (ct.Buyer.Name) ]
                            dd [ ClassName "col-9 offset-3" ] [ text (ct.Buyer.Representative.Name) ]
                        ]
                    ]

                    dd [ ClassName "col-6" ] [
                        dl [ ClassName "row"] [
                            dt [ ClassName "col-3" ] [ text "Load From: " ]
                            dd [ ClassName "col-9" ] [ text (ct.TerminalLoad.Name) ]
                            dd [ ClassName "col-9 offset-3" ] [ dateView ct.DateLoad ]
                        ]
                    ]
                    dd [ ClassName "col-6" ] [
                        dl [ ClassName "row"] [
                            dt [ ClassName "col-3" ] [ text "Discharge: " ]
                            dd [ ClassName "col-9" ] [ text (ct.TerminalDischarge.Name) ]
                            dd [ ClassName "col-9 offset-3" ] [ dateView ct.DateDischarge ]
                        ]
                    ]
                ]
            ]

        override __.RenderBody () = 
            match __.props.state with
            | Created state -> signedView contractTermsView state.Deal
            | SentForAgreement state -> signedView contractTermsView state.Deal
            | BuyerRejected state -> buyerRejectedView state.RejectedDeal 
            | BuyerAccepted state -> counterSignedDealView state.CounterSignedDeal
            | VesselRequested state -> vesselRequestedView state.CounterSignedDeal state.Request
            | VesselOffered state -> vesselOfferedView state.CounterSignedDeal state.Request state.Offers
            | VesselNominated state -> vesselNominationView state.Nomination
            | VesselNominationAccepted state -> signedView vesselNominationView state.AcceptedNomination
            | VesselNominationRejected state -> vesselNominationRejectedView state.RejectedNomination
            | NORReleased state -> norReleasedView state.NOR state.Nomination
            | LoadBoLCreated state -> departureInfoView state.DepartureInfo
            | InTransitOrUnpaid state -> 
                let phys = match state.Physical with 
                            | VesselDeparted state -> deliveryProgressView state.Progress
                            | VesselArrived state -> arrivalInfoView state.ArrivalInfo
                            | InspectionPerformed state -> inspectionView state.InspectionResult
                            | DischargeDocumentsCreated state -> dischargeDocumentsCreatedView state
                let fin = match state.Financial with 
                            | ReadyForInvoicing state -> invoiceDataView state.InvoiceData
                            | InvoiceCreated state -> signedView invoiceDataView state.InvoiceData
                            | PaymentMade state -> paymentMadeStateView state
                dl [] [
                    fieldView 2 "Physical Progress" phys
                    fieldView 2 "Invoicing Progress" fin
                    fieldView 2 "Departure Info" (signedView departureInfoView state.DepartureInfo)
                ]
            | DeliveredAndPaid (di, ds, ps) -> //di.V.V.Nomination.CounterSignedDeal.V.V
                dl [] [
                    fieldView 2 "Physical Progress" (dischargeDocumentsCreatedView ds)
                    fieldView 2 "Invoicing Progress" (paymentMadeStateView ps)
                    fieldView 2 "Departure Info" (signedView departureInfoView di)
                ]
            // | _ -> pre [] [ __.props.state?data |> toJsonPretty 2. |> text ]
            
    let contractStateControl state = com<ContractStateControl, _, _>({ state = state }) []


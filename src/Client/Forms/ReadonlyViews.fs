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


module ReadonlyViews =

    type [<Pojo>] ImageDecorator = {
        src: string
        alt: string option
    }

    type [<Pojo>] ImageViewerProps = {
        visible: bool
        images: ImageDecorator[]
        onClose: unit -> unit
        zIndex: int option
    }
    
    let imageViewer: ImageViewerProps -> React.ReactElement = importDefault "react-viewer"


    let individualView i = 
        dl [ ClassName "row"] [
            dd [ ClassName "col-4" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-4" ] [ text "First Name: " ]
                    dd [ ClassName "col-8" ] [ text i.FirstName ]
                ]
            ]
            dd [ ClassName "col-4" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-4" ] [ text "Middle Name: " ]
                    dd [ ClassName "col-8" ] [ text i.MiddleName ]
                ]
            ]
            dd [ ClassName "col-4" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-4" ] [ text "Last Name: " ]
                    dd [ ClassName "col-8" ] [ text i.LastName ]
                ]
            ]
            dd [ ClassName "col-6" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-3" ] [ text "Phone: " ]
                    dd [ ClassName "col-9" ] [ text i.PhoneNumber ]
                ]
            ]
            dd [ ClassName "col-6" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-3" ] [ text "Email: " ]
                    dd [ ClassName "col-9" ] [ text i.Email ]
                ]
            ]
        ]

    let representativeView (i: Individual) = 
        dl [ ClassName "row"] [
            dd [ ClassName "col-12" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-3" ] [ text "Representative: " ]
                    dd [ ClassName "col-9" ] [ text i.Name ]
                ]
            ]
            dd [ ClassName "col-12" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-2" ] [ text "Phone: " ]
                    dd [ ClassName "col-4" ] [ text i.PhoneNumber ]

                    dt [ ClassName "col-2" ] [ text "Email: " ]
                    dd [ ClassName "col-4" ] [ text i.Email ]
                ]
            ]
        ]

    let legalEntityView (le: LegalEntity) = 
        dl [ ClassName "row"] [
            dd [ ClassName "col-5" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-3" ] [ text "Name: " ]
                    dd [ ClassName "col-9" ] [ text le.Name ]
                ]
            ]
            dd [ ClassName "col-7" ] [
                dl [ ClassName "row"] [
                    dt [ ClassName "col-3" ] [ text "Address: " ]
                    dd [ ClassName "col-9" ] [ text le.Address ]
                ]
            ]
        ]

    let partyView p = dl [ ClassName "row" ] [
                        dd [ ClassName "col-11 offset-1" ] [
                            dl [] [
                                // dt [] [ text "Legal Entity:" ]
                                dd [] [ legalEntityView p.LegalEntity ]
                            ]
                        ]
                        dd [ ClassName "col-11 offset-1" ] [
                            dl [] [
                                // dt [] [ text "Representative:" ]
                                dd [] [ representativeView p.Representative ]
                            ]
                        ]
                    ]


    let organizationView = 
        function
        | Ltd l ->  partyView l
        | Corporate c -> partyView c.Headquaters

    let volumeView (v: Volume) = 
        text (!!v?data + " " + getUnionCaseNames<Volume>.[!!v?tag]) 

    let qualitySpecsView (qs: QualitySpecs) = 
        dl [ ClassName "row" ] [
            dt [ ClassName "col-2" ] [ text ("Density: ") ]
            dd [ ClassName "col-4" ] [ text qs.Density ]

            dt [ ClassName "col-2" ] [ text ("Sulphur Level: ") ]
            dd [ ClassName "col-4" ] [ text qs.SulphurLevel ]
        ]

    let dateView (dt: DateTime) = text (dt.Date.ToString("dd MMM yyyy"))
    let contractTypeView (ct: ContractType) = text (getUnionCaseName ct)

    let fieldView i l re = 
        dl [ ClassName "row" ] [
            dt [ ClassName ("col-" + i.ToString()) ] [ text (l + ": ") ]
            dd [ ClassName ("col-" + (12 - i).ToString()) ] [ re ]
        ]

    let fieldView2 l re = 
        [
            dt [] [ text (l + ": ") ]
            dd [] re
        ]

    let contractTermsView (ct: ContractTerms) = 
        [
            fieldView2 "Seller" [ organizationView ct.Seller ]
            fieldView2 "Buyer" [ organizationView ct.Buyer ]
            fieldView2 "Inspector" [ organizationView ct.Inspector ]
            fieldView2 "Load" [ fieldView 2 "Terminal" (organizationView ct.TerminalLoad)
                                fieldView 2 "Date" (dateView ct.DateLoad) ]
            fieldView2 "Discharge" [fieldView 2 "Terminal" (organizationView ct.TerminalDischarge)
                                    fieldView 2 "Date" (dateView ct.DateDischarge) ]
            [   dd [ ClassName "col-6" ] [ fieldView 2 "Price" (text ct.Price) ]
                dd [ ClassName "col-6" ] [ fieldView 2 "Quantity" (volumeView ct.Quantity) ]
            ]
            
            fieldView2 "Quality Specs" [ qualitySpecsView ct.QualitySpecs ]
            fieldView2 "Type of Contract" [ contractTypeView ct.ContractType ]
        ]   |> List.collect id 
            |> dl []


    type [<Pojo>] SignedViewProps = { caption: string; signature: Signature }
    type SignedView(props) =
        inherit Collapsable<SignedViewProps, obj>(props, (fun _ -> createObj[]), props.caption, "btn-link")

        [<PassGenerics>]
        override __.RenderForm collapseButton = 
            let s = __.props.signature
            let hash = s.Sig.ToString()
            let ttId = "tt" + hash
            Browser.console.log s
            div [] [
                collapseButton
                a [ Href "#"
                    Id ttId
                    ] [ text "signature verified" ]
                UncontrolledTooltip (fun p ->   p.placement <- PlacementEnum.Top |> Some
                                                p.target <- ttId |> Some) [ 
                                                        fieldView 3 "Role" (text (s.Role.ToString() |> splitOnCapital)) 
                                                        fieldView 3 "Signer" (text s.Signer) 
                                                        fieldView 3 "Sig" (text hash) 
                                                 ]
            ]

        override __.RenderBody () = div [] [!!__.children]


    let [<PassGenerics>] inline signedView<'T, 'S when 'S: (member Name: string)> (innerView: 'T -> React.ReactElement) (v: Signed<'T, 'S>) =
        com<SignedView, _, _>({ caption = v.Sig.Signer; signature = v.Sig }) [ (innerView v.V) ]

    let inline counterSignedDealView (csd: CounterSignedDeal) = signedView (signedView contractTermsView) csd
            
    let inline buyerRejectedView (s: BuyerSigned<DealToSchedule * RejectionReason>) = 
        let rs = fun (deal, reason) -> 
            dl [] [   
                dd [ ClassName "col-12" ] [ fieldView 5 "Rejection reason" (text (reason.Reason)) ]
                dd [ ClassName "col-12" ] [ signedView contractTermsView deal ]
            ]
        signedView rs s

    let inline vesselRequestView vesselRequest = 
        let vr = fun vr -> [    fieldView2 "Capacity" [ volumeView vr.Capacity ]
                                [   dd [ ClassName "col-6" ] [ fieldView 2 "Load" (dateView vr.DepartureDate) ]
                                    dd [ ClassName "col-6" ] [ fieldView 2 "From" (text vr.Terminal.Name) ]

                                    dd [ ClassName "col-6" ] [ fieldView 2 "Load" (dateView vr.ArrivalDate) ]
                                    dd [ ClassName "col-6" ] [ fieldView 2 "From" (text vr.Destination.Name) ]
                                ]
                            ]   |> List.collect id 
                                |> dl []
        signedView vr vesselRequest

    let [<PassGenerics>] inline vesselRequestedView csd vesselRequest = 
        vesselRequestView vesselRequest

    let [<PassGenerics>] inline vesselOfferView (offer: VesselOperatorSigned<VesselOffer>) = 
        let vov = fun (vo: VesselOffer) ->
            [   fieldView 4 "Vessel" ( text (vo.Vessel.ToString()) )
                fieldView 4 "Price" ( text (vo.Price) )
                fieldView 4 "Available" ( dateView (vo.AvailabilityDate) )
            ]   //|> List.collect id 
                |> div []
        signedView vov offer

    let [<PassGenerics>] inline vesselOfferedView csd vesselRequest (offers: VesselOperatorSigned<VesselOffer> list) = 
        offers |> List.map (fun svo -> dd [] [ vesselOfferView svo ] ) |> dl []

    let vesselNominationView (vn: VesselNomination) =
        [   fieldView2 "Deal" [ counterSignedDealView vn.CounterSignedDeal ]
            fieldView2 "Nominated Vessel" [ signedView vesselOfferView vn.NominatedVessel ]
        ]   |> List.collect id 
            |> dl []

    let [<PassGenerics>] vesselNominationRejectedView (vnr: BuyerSigned<VesselNomination * RejectionReason>) =
        let rs = fun (vn, reason) -> 
            dl [] [   
                dd [ ClassName "col-12" ] [ fieldView 5 "Rejection reason" (text (reason.Reason)) ]
                dd [ ClassName "col-12" ] [ vesselNominationView vn ]
            ]
        signedView rs vnr

    let [<PassGenerics>] norView (snor: CaptainSigned<NOR>) =
        let nor = fun (nor: NOR) -> 
            dl [] [   
                dd [ ClassName "col-6" ] [ fieldView 2 "Departure date" ( dateView (nor.Date) ) ]
                dd [ ClassName "col-6" ] [ fieldView 2 "Vessel" ( text (nor.Vessel.ToString()) ) ]
                dd [ ClassName "col-6" ] [ fieldView 2 "Quantity" ( volumeView nor.Quantity ) ]
                dd [ ClassName "col-6" ] [ fieldView 2 "Terminal" ( text (nor.Terminal.Name) ) ]
            ]
        signedView nor snor

    let [<PassGenerics>] inline norReleasedView (snor: CaptainSigned<NOR>) (vn: VesselNomination) =
        [   fieldView2 "Notice of Readiness" [ norView snor ]
            [ dd [] [ vesselNominationView vn ] ]
        ]   |> List.collect id 
            |> dl []

    type [<Pojo>] SupportingDocumentsProps = { value: SupportingDocuments }
    type [<Pojo>] SupportingDocumentsState = { visible: bool option; activeIndex: int }
    type SupportingDocumentsView(props: SupportingDocumentsProps) =
        inherit React.Component<SupportingDocumentsProps, SupportingDocumentsState>(props)
        do base.setInitState({ visible = None; activeIndex = 0 })

        [<PassGenerics>]
        member __.render () =
            div [] [
                div [] (__.props.value |> List.mapi (fun i d -> img [   Src (d.Data)
                                                                        OnClick (fun _ -> __.setState({ __.state with visible = Some true; activeIndex = i }))
                                                                        imgItemCss ]))
                div [] [
                    fn imageViewer ({   visible = match __.state.visible with None -> false | Some v -> v 
                                        images = __.props.value |> List.map (fun d -> { src = d.Data; alt = None } ) |> List.toArray
                                        zIndex = Some 2000
                                        onClose = fun () -> __.setState({ __.state with visible = Some false }) }) []
                ]
            ]

    let supportingDocumentsView supportingDocuments = com<SupportingDocumentsView, _, _> { value = supportingDocuments } []

    let bolView (bol: BillOfLading) =
        div [] [
            dl [ ClassName "row" ] [   
                    dd [ ClassName "col-6" ] [ fieldView 2 "Cargo Reference Number" ( text (bol.CargoReferenceNumber) ) ]
                    dd [ ClassName "col-6" ] [ fieldView 2 "BoL Number" ( text (bol.BoLNumber) ) ]
                    dd [ ClassName "col-6" ] [ fieldView 2 "Quantity" ( volumeView bol.Quantity ) ]
                    dd [ ClassName "col-6" ] [ fieldView 2 "Vessel" ( text (bol.Carrier.ToString()) ) ]
                    dd [ ClassName "col-6" ] [ fieldView 2 "Seller" ( text (bol.Seller.Name) ) ]
                    dd [ ClassName "col-6" ] [ fieldView 2 "Buyer" ( text (bol.Buyer.Name) ) ]
                ]
            supportingDocumentsView bol.SupportingDocuments
        ]


    let [<PassGenerics>] inline departureInfoView (svdi: TerminalSigned<VesselDepartureInfo>) = 
        let vdi = fun (vdi: VesselDepartureInfo) -> 
            dl [] [   
                dd [ ClassName "col-12" ] [ bolView vdi.BillOfLading ]
                dd [ ClassName "col-12" ] [ vesselNominationView vdi.Nomination ]
            ] 
        signedView vdi svdi

    let deliveryProgressView (progress: CaptainSigned<DeliveryProgress> option) =
        match progress with 
        | Some prg -> signedView (fun p -> (fieldView2 "Delivery Progress" [text p.Progress]).Head) prg
        | None -> text ""

    let arrivalInfoView (arrivalInfo: CaptainSigned<ArrivalInfo>) =
        signedView (fun ai -> (fieldView2 "Arrival Info" [dateView ai.Date]).Head) arrivalInfo

    let inspectionView (inspectionResult: InspectorSigned<InspectionResult>) =
        let ir = fun (ir: InspectionResult) -> 
            dl [] [   
                dd [ ClassName "col-6" ] [ fieldView 2 "Actual delivery date" ( dateView (ir.ActualDeliveryDate) ) ]
                dd [ ClassName "col-6" ] [ fieldView 2 "Quantity" ( volumeView ir.Quantity ) ]
                dd [ ClassName "col-12" ] [ fieldView 2 "Quality" ( qualitySpecsView ir.QualitySpecs ) ]
            ]
        signedView ir inspectionResult        

    let invoiceDataView (invoiceData: InvoiceData) =
        [   fieldView2 "Invoice date" [ dateView invoiceData.Date ]
            fieldView2 "Departure info" [ departureInfoView (invoiceData.DepartureInfo) ]
        ]   |> List.collect id 
            |> dl []

    let settlementInfoView (si: SettlementInfo) =
        dl [ ClassName "row" ] [   
            dd [ ClassName "col-6" ] [ fieldView 2 "Payment date" ( dateView (si.PayDate) ) ]
            dd [ ClassName "col-6" ] [ fieldView 2 "Settlement date" ( dateView (si.SettlementDate) ) ]
        ]

    let paymentMadeStateView (pms: PaymentMadeState) =
        dl [ ClassName "row" ] [   
            dd [ ClassName "col-6" ] [ fieldView 2 "Settlement date" ( signedView settlementInfoView (pms.SettlementInfo) ) ]
            dd [ ClassName "col-6" ] [ fieldView 2 "Invoice" ( signedView invoiceDataView (pms.InvoiceData) ) ]
        ]
    
    let dischargeDocumentsCreatedView state = signedView (fun (dd: DischargeDocuments) -> bolView dd.BillOfLading) state.DischargeDocuments
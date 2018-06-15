namespace ServerCode
module Commodities =
 
    open System
    open ServerCode.Utils

    type UID = uint32

    type Party = {
        Uid: UID
        LegalEntity: LegalEntity
        Representative: Individual 
    }
    and LegalEntity = {
        Name: string
        Address: Address
        // TODO: Contact# etc
        //Contacts: Individual list
    }
    and Address = string // TODO: Expand this
    and Individual = {
        Uid: UID
        FirstName: string
        LastName: string
        MiddleName: string 
        PhoneNumber: string
        Email: string
        // TODO: Contact info etc
    }
        with member __.Name = __.FirstName + " " + (if String.IsNullOrWhiteSpace(__.MiddleName) then "" else __.MiddleName + " ") + __.LastName
    and Organization =
    | Ltd of Party
    | Corporate of Corporate
        with 
            member __.Uid = match __ with | Ltd p -> p.Uid | Corporate c -> c.Headquaters.Uid
            member __.Name = match __ with | Ltd p -> p.LegalEntity.Name | Corporate c -> c.Name
            member __.Representative = match __ with | Ltd p -> p.Representative | Corporate c -> c.Headquaters.Representative
    and Corporate = {
        Name: string // TODO: Probably defer it from Headquaters
        Headquaters: Party
        //Subordinaries: Organization list // TODO: Re-add subs back. Were commented out because of an issue with serialization for Swagger docs (it doesn't like recursion unfortunatelly)
    }


    type Buyer = Organization
    type Seller = Organization
    type Trader = {
        Seller: Seller
        Buyer: Buyer
    }
    with 
        member __.Uid = __.Seller.Uid
        member __.Name = __.Seller.Name

    type Inspector = Organization
    type Insurer = Organization

    type Captain = Individual

    type Terminal = Organization

    type VesselOperator = Organization

    [<RequireQualifiedAccess>]
    type SignerRole =
    | Seller
    | Buyer
    | VesselOperator
    | TerminalLoad
    | TerminalDischarge
    | Inspector
    | Captain
    with member __.ToCaption = getUnionCaseName __ 

    type BusinessActor<'details when 'details: (member Name: string)> = {
        Details: 'details
    }

    type Signature = { 
        Role: SignerRole
        Signer: string
        SignerUID: UID
        Sig: Sig // TODO: either hash or digital signature or whatever
    }
    and Signed<'T, 'S when 'S: (member Name: string)>= {
        Sig: Signature
        V: 'T
    }
    and SellerSignature = Signature
    and SellerSigned<'T> = Signed<'T, Seller>
    
    and BuyerSignature = Signature
    and BuyerSigned<'T> = Signed<'T, Buyer>

    and TerminalSignature = Signature
    and TerminalSigned<'T> = Signed<'T, Terminal>

    and CaptainSignature = Signature
    and CaptainSigned<'T> = Signed<'T, Captain>

    and InspectorSignature = Signature
    and InspectorSigned<'T> = Signed<'T, Inspector>

    and VesselOperatorSignature = Signature
    and VesselOperatorSigned<'T> = Signed<'T, VesselOperator>

    and Sig = Sig of string
    with override __.ToString() = match __ with Sig h -> h

    type CargoReferenceNumber = string
    type BoLNumber = string

    type [<Measure>]percent
    type [<Measure>]degrees
    type [<Measure>]kg_m3
    type [<Measure>]usd

    type Volume =
    | Barrel of float<barrels> 
    | Mt of float<mts>
    | Btus of float<btus>
    | Piece of float<peices>
    | TonneVac of float<tonnesVac>
    | TonneAir of float<tonnesAir>
    | Cubes of float
    and [<Measure>]barrels
    and [<Measure>]mts
    and [<Measure>]btus
    and [<Measure>]peices
    and [<Measure>]tonnesVac
    and [<Measure>]tonnesAir

    and QualitySpecs = { // TODO: Change to Euro1|Euro2|Euro3|Euro4|Euro5|Euro6 ?
        Density: Density
        SulphurLevel: SulphurLevel
    }
    and APIGravity = float<degrees> // TODO: Check with the Business if it applies to Oil Products
    and Density = float<kg_m3>
    and SulphurLevel = float<percent>
    and Price = float<usd>

    and SupportingDocuments = SupportingDocument list 
    and SupportingDocument = {
        Name: string
        Data: string // Supporting documents serialized and encoded as Base64 strings
    }

    type Vessel = {
        Name: string
        Capacity: Volume 
        //Operator: VesselOperator
        IMO: uint32 option
        MMSI: uint32
        CallSign: string
        Flag: string
        VesselType: VesselType 
        Built: uint16 option
        Captain: Captain
    } with 
        override __.ToString() = __.Name + " | " + __.Capacity.ToString() + " | " + __.Flag
    and VesselOperatorFleet = {
        Operator: VesselOperator
        Fleet: Vessel list 
    }
    and VesselType = 
    | Barge 
    | CrudeOilTanker 
    | LpgCarrier 
    | AsphaltTanker 
    | ContainerShip


    type ContractType = FOB | CIF 

    ////// I. Transaction Execution
    type ContractTerms = {
        ContractType: ContractType
        Seller: Seller
        Buyer: Buyer
        Inspector: Inspector
        Price: Price
        QualitySpecs: QualitySpecs
        Quantity: Volume
        TerminalLoad: Terminal
        TerminalDischarge: Terminal
        DateLoad: DateTime
        DateDischarge: DateTime
        Terms: string array // TODO: Rethink this
    }
    and RejectionReason = { 
        Reason: string 
    }

    type DealToSchedule = SellerSigned<ContractTerms>
    and CounterSignedDeal = BuyerSigned<DealToSchedule>

    and BillOfLading = {
        CargoReferenceNumber: CargoReferenceNumber
        Seller: Seller
        Buyer: Buyer
        Carrier: Vessel
        BoLNumber: BoLNumber
        Quantity: Volume
        SupportingDocuments: SupportingDocuments
    }

    and NOR = {
        Terminal: Terminal
        Date: DateTime
        Vessel: Vessel
        Quantity: Volume // TODO: Type of product? Other info?
    }
    and InspectionResult = {
        ActualDeliveryDate: DateTime
        QualitySpecs: QualitySpecs
        Quantity: Volume
    }

    // TODO: Currently not in use types
    // TODO: Decide if they should be kept or deleted
    module MaybeNeededInTheFuture =
        type Shipment = Vessel * DealToSchedule

        type MasterInsuranceAgreement = MasterInsuranceAgreementRef of string

        type ArrangeInsurance = Shipment -> InsuranceContract
        and InsuranceContract = {
            Insurer: Insurer
            Coverage: float<usd>
            MasterAgreement: MasterInsuranceAgreement
            // TODO: Add more details
        }

        type CaptureCargoInformation = Shipment -> InspectionResult -> BillOfLading

        type PreparedDeal = {
            Shipment: Shipment
            BillOfLading: BillOfLading
            SecondaryCosts: SecondaryCosts
        }
        and SecondaryCosts = {
            PortCosts: (Comment * Cost) option
            InsuranceCosts: (Comment * Cost) option
        }
        and Comment = Comment of string
        and Cost = Cost of float<usd>

        and Instructions = InspectorInstructions * CustomsInstructions * ExpeditorInstructions
        and InspectorInstructions = InspectorInstructions // TODO: Add fields
        and CustomsInstructions = CustomsInstructions     // TODO: Add fields
        and ExpeditorInstructions = ExpeditorInstructions // TODO: Add fields

        type Port = {
            Name: string
            LOCODE: string list
            Terminals: Terminal list
            Tide: float<m>
            ChannelDepth: float<m>
            AnchorageDepth: float<m>
            CargoPierDepth: float<m>
            OilTerminalDepth: float<m>
        }
        and [<Measure>]m


    type DischargeDocuments = {
        BillOfLading: BillOfLading
    } // TODO: Define other fields

    let private rnd = Random()

    /////// SellerContract

    type SellerContract = {
        State: SellerContractState
        History: History
    }
    with 
        member __.Audit (msg: TradingMsg) (newState: SellerContractState) = // TODO: Add signature extruction of message
            { State = newState; History = { State = newState; Msg = Some msg; Timestamp = DateTime.Now } :: __.History }
        static member New uid deal = 
            let state = { CreatedState.Deal = deal; UID = uid } |> Created  
            { State = state; History = [ { State = state; Msg = None; Timestamp = DateTime.Now } ] }
    and SellerContractState =
    | Created of CreatedState
    | SentForAgreement of SentForAgreementState
    | BuyerRejected of BuyerRejectedState
    | BuyerAccepted of BuyerAcceptedState 
    | VesselRequested of VesselRequestedState
    | VesselOffered of VesselOfferedState
    | VesselNominated of VesselNominatedState
    | VesselNominationAccepted of VesselNominationAcceptedState
    | VesselNominationRejected of VesselNominationRejectedState
    | NORReleased of ``NOR Released State``
    | LoadBoLCreated of LoadBoLCreatedState // TODO: Add Additional Seller Documents Action/State
    | InTransitOrUnpaid of InTransitOrUnpaidState
    | DeliveredAndPaid of ArchivedState
    with member __.StateName = match __ with 
                                 | InTransitOrUnpaid s -> 
                                    (s.Physical |> getUnionCaseNameSplit, s.Financial |> getUnionCaseNameSplit).ToString()
                                 | _ -> __ |> getUnionCaseNameSplit

    and HistoryStep = {
        State: SellerContractState
        Msg: TradingMsg option
        Timestamp: DateTime
    }
    and History = HistoryStep list

    and StateChanger = SellerContract -> SellerContract

    and IHasUidAndSigned = 
        abstract UID: UID
        abstract Signature: Signature

    and TradingMsg = 
      | SellerMsg of SellerMsg
      | BuyerMsg of BuyerMsg
      | VesselOperatorMsg of VesselOperatorMsg
      | TerminalLoadMsg of TerminalLoadMsg
      | TerminalDischargeMsg of TerminalDischargeMsg
      | InspectorMsg of InspectorMsg
      | CaptainMsg of CaptainMsg
      with
        member private __.ToUidAndSigned = match __ with 
                                            | SellerMsg m -> m :> IHasUidAndSigned
                                            | BuyerMsg m -> m :> IHasUidAndSigned
                                            | VesselOperatorMsg m -> m :> IHasUidAndSigned
                                            | TerminalLoadMsg m -> m :> IHasUidAndSigned
                                            | TerminalDischargeMsg m -> m :> IHasUidAndSigned
                                            | InspectorMsg m -> m :> IHasUidAndSigned
                                            | CaptainMsg m -> m :> IHasUidAndSigned
        member __.UID = __.ToUidAndSigned.UID
        member __.Signature =__.ToUidAndSigned.Signature
    and SellerMsg = 
      | AddContract of UID * DealToSchedule
      | SendForAgreement of UID * DealToSchedule
      | RequestVessel of UID * SellerSigned<VesselRequest>
      | NominateVessel of UID * NominatedVessel
      | NominateAnotherVessel of UID * NominatedVessel
      | RequestAnotherVessel of UID * SellerSigned<VesselRequest>
      | ResendForAgreement of UID * DealToSchedule
      | IssueInvoice of UID * SellerSigned<InvoiceData>
      interface IHasUidAndSigned with
        override __.UID = match __ with 
                            | AddContract (uid, _) -> uid 
                            | SendForAgreement (uid, _) | RequestVessel (uid, _) | NominateVessel (uid, _) | NominateAnotherVessel (uid, _) 
                            | RequestAnotherVessel (uid, _) | ResendForAgreement (uid, _) | IssueInvoice (uid, _) -> uid
        override __.Signature = match __ with 
                                | AddContract (_, (s: Signed<_, _>)) -> s.Sig
                                | SendForAgreement (_, (s: Signed<_, _>)) -> s.Sig
                                | RequestVessel (_, (s: Signed<_, _>)) -> s.Sig
                                | NominateVessel (_, (s: Signed<_, _>)) -> s.Sig
                                | NominateAnotherVessel (_, (s: Signed<_, _>)) -> s.Sig
                                | RequestAnotherVessel (_, (s: Signed<_, _>)) -> s.Sig
                                | ResendForAgreement (_, (s: Signed<_, _>)) -> s.Sig
                                | IssueInvoice (_, (s: Signed<_, _>)) -> s.Sig
    and BuyerMsg = 
      | BuyerAccept of UID * BuyerSigned<DealToSchedule> 
      | BuyerReject of UID * BuyerSigned<DealToSchedule * RejectionReason>
      | AcceptNomination of UID * BuyerSigned<VesselNomination> 
      | RejectionNomination of UID * BuyerSigned<VesselNomination * RejectionReason>
      | PayInvoice of UID * BuyerSigned<SettlementInfo>
      interface IHasUidAndSigned with
        override __.UID = match __ with 
                            | BuyerAccept (uid, _) | BuyerReject (uid, _) | AcceptNomination (uid, _) | RejectionNomination (uid, _) 
                            | PayInvoice (uid, _) -> uid
        override __.Signature = match __ with 
                                | BuyerAccept (_, (s: Signed<_, _>)) -> s.Sig
                                | BuyerReject (_, (s: Signed<_, _>)) -> s.Sig
                                | AcceptNomination (_, (s: Signed<_, _>)) -> s.Sig
                                | RejectionNomination (_, (s: Signed<_, _>)) -> s.Sig
                                | PayInvoice (_, (s: Signed<_, _>)) -> s.Sig
    and VesselOperatorMsg = 
      | OfferVessel of UID * VesselOperatorSigned<VesselOffer>
      interface IHasUidAndSigned with
        override __.UID = match __ with | OfferVessel (uid, _) -> uid
        override __.Signature = match __ with | OfferVessel (_, (s: Signed<_, _>)) -> s.Sig
    and TerminalLoadMsg = 
      | CreateLoadBoL of UID * TerminalSigned<VesselDepartureInfo>
      interface IHasUidAndSigned with
        override __.UID = match __ with | CreateLoadBoL (uid, _) -> uid
        override __.Signature = match __ with | CreateLoadBoL (_, (s: Signed<_, _>)) -> s.Sig
    and TerminalDischargeMsg = 
      | ReleaseDischargeDocuments of UID * TerminalSigned<DischargeDocuments>
      interface IHasUidAndSigned with
        override __.UID = match __ with | ReleaseDischargeDocuments (uid, _) -> uid
        override __.Signature = match __ with | ReleaseDischargeDocuments (_, (s: Signed<_, _>)) -> s.Sig    
    and InspectorMsg =
      | PerformInspection of UID * InspectorSigned<InspectionResult>
      interface IHasUidAndSigned with
        override __.UID = match __ with | PerformInspection (uid, _) -> uid
        override __.Signature = match __ with | PerformInspection (_, (s: Signed<_, _>)) -> s.Sig
    and CaptainMsg = 
      | ReleaseNOR of UID * CaptainSigned<NOR>
      | DepartVessel of UID * CaptainSigned<TerminalSigned<VesselDepartureInfo>>
      | NotifyOfVesselProgress of UID * CaptainSigned<DeliveryProgress>
      | ArriveVessel of UID * CaptainSigned<ArrivalInfo>
      interface IHasUidAndSigned with
        override __.UID = match __ with | ReleaseNOR (uid, _) | DepartVessel (uid, _) | NotifyOfVesselProgress (uid, _) | ArriveVessel (uid, _) -> uid
        override __.Signature = match __ with 
                                | ReleaseNOR (_, (s: Signed<_, _>)) -> s.Sig
                                | DepartVessel (_, (s: Signed<_, _>)) -> s.Sig
                                | NotifyOfVesselProgress (_, (s: Signed<_, _>)) -> s.Sig
                                | ArriveVessel (_, (s: Signed<_, _>)) -> s.Sig

    and CreatedState = {
        UID: UID
        Deal: DealToSchedule
    } 
    with 
        member __.``Send For Agreement``() = 
            { SentForAgreementState.Deal = __.Deal } |> SentForAgreement 

    and SentForAgreementState = { // TODO: Placeholder for an internal approval
        Deal: DealToSchedule
    } 
    with 
        member __.Accept counterSignedDeal = 
            { BuyerAcceptedState.CounterSignedDeal = counterSignedDeal } |> BuyerAccepted
        member __.Reject rejectedDeal = 
            { RejectedDeal = rejectedDeal } |> BuyerRejected

    and BuyerRejectedState = {
        RejectedDeal: BuyerSigned<DealToSchedule * RejectionReason>
    } 
    with 
        member __.``Resend For Agreement`` newDeal = 
            { SentForAgreementState.Deal = newDeal } |> SentForAgreement
         

    and BuyerAcceptedState = {
        CounterSignedDeal: CounterSignedDeal
    } 
    with 
        member __.``Request Vessel`` signedVesselRequest = 
            let ct = __.CounterSignedDeal.V.V
            {   VesselRequestedState.CounterSignedDeal = __.CounterSignedDeal
                Request = signedVesselRequest } |> VesselRequested
    and VesselRequest = {
        Capacity: Volume
        Terminal: Terminal
        Destination: Terminal
        DepartureDate: DateTime
        ArrivalDate: DateTime
    } 

    and VesselRequestedState = {
        CounterSignedDeal: CounterSignedDeal
        Request: SellerSigned<VesselRequest>
    } 
    with 
        member __.``Offer a Vessel`` signedVesselOffer = 
            
            {   CounterSignedDeal = __.CounterSignedDeal
                VesselOfferedState.Request = __.Request
                Offers = [ signedVesselOffer ] } |> VesselOffered
    and VesselOffer = { 
        Vessel: Vessel 
        Price: Price 
        AvailabilityDate: DateTime
    }
    and NominatedVessel = SellerSigned<VesselOperatorSigned<VesselOffer>>

    and VesselOfferedState = {
        CounterSignedDeal: CounterSignedDeal
        Request: SellerSigned<VesselRequest>
        Offers: VesselOperatorSigned<VesselOffer> list
    } 
    with 
        member __.``Offer a Vessel`` signedVesselOffer = 
            { __ with Offers = signedVesselOffer :: __.Offers } |> VesselOffered
        member __.``Nominate Vessel`` (nominatedVessel: NominatedVessel)  = // TODO: Take the deal from the history?
            let vesselOffer = nominatedVessel.V
            if __.Offers |> List.contains vesselOffer |> not then failwith "Unknown vessel offer" // TODO: If the seller should not be constrained to pick only offered vessels remove this check
            {   VesselNominatedState.Nomination = { VesselNomination.CounterSignedDeal = __.CounterSignedDeal
                                                    NominatedVessel = nominatedVessel } 
                VesselOffers = __.Offers } |> VesselNominated
    and VesselNomination = {
        CounterSignedDeal: CounterSignedDeal
        NominatedVessel: NominatedVessel
    } 

    and VesselNominatedState = {
        Nomination: VesselNomination
        VesselOffers: VesselOperatorSigned<VesselOffer> list
    } 
    with 
        member __.``Accept Nomination`` (signedNomination: BuyerSigned<VesselNomination>) = 
            { VesselNominationAcceptedState.AcceptedNomination = signedNomination } |> VesselNominationAccepted
        member __.``Reject Nomination`` (rejectedNomination: BuyerSigned<VesselNomination * RejectionReason>) = 
            {   VesselNominationRejectedState.RejectedNomination = rejectedNomination
                VesselOffers = __.VesselOffers } |> VesselNominationRejected

    and VesselNominationRejectedState = {
        RejectedNomination: BuyerSigned<VesselNomination * RejectionReason>
        VesselOffers: VesselOperatorSigned<VesselOffer> list
    } 
    with 
        member __.``Nominate Another Vessel`` (nominatedVessel: NominatedVessel) = // TODO: Take the deal from the history?
            {   VesselNominatedState.Nomination = { CounterSignedDeal = (__.RejectedNomination.V |> fst).CounterSignedDeal
                                                    NominatedVessel = nominatedVessel }
                VesselOffers = __.VesselOffers} |> VesselNominated
        member __.``Request Another Vessel`` svr = 
            let csd = (__.RejectedNomination.V |> fst).CounterSignedDeal
            {   VesselRequestedState.CounterSignedDeal = csd; 
                Request = svr } |> VesselRequested
        member __.``Resend For Agreement`` newDeal = { SentForAgreementState.Deal = newDeal } |> SentForAgreement

    and VesselNominationAcceptedState = {
        AcceptedNomination: BuyerSigned<VesselNomination>
    } 
    with 
        member __.``Notice of Readiness`` (nor: CaptainSigned<NOR>) = 
            // TODO: Add signature and nomination check
            let nomination = __.AcceptedNomination.V
            let offer = nomination.CounterSignedDeal.V
            { Nomination = nomination; NOR = nor } |> NORReleased

    and ``NOR Released State`` = {
        Nomination: VesselNomination
        NOR: CaptainSigned<NOR>
    } 
    with 
        member __.CreateLoadBoL (vesselDepartureInfo: TerminalSigned<VesselDepartureInfo>) = 
            let offer = __.Nomination.CounterSignedDeal.V
            {   LoadBoLCreatedState.DepartureInfo = vesselDepartureInfo } |> LoadBoLCreated
    and BoLInfo = {
        CargoReferenceNumber: CargoReferenceNumber
        BoLNumber: BoLNumber
        Quantity: Volume
        SupportingDocuments: SupportingDocuments
    }

    and LoadBoLCreatedState = {
        DepartureInfo: TerminalSigned<VesselDepartureInfo>
    } 
    with 
        member __.``Depart Vessel`` departureInfo = 
            // TODO: Add signature and DepartureInfo check
            {   DepartureInfo = departureInfo
                Physical = { DepartureInfo = departureInfo; Progress = None } |> VesselDeparted
                Financial = { ReadyForInvoicingState.InvoiceData = { InvoiceData.DepartureInfo = __.DepartureInfo; Date = DateTime.Now } } |> ReadyForInvoicing
                 } |> InTransitOrUnpaid
    
    and VesselDepartureInfo = {
        Nomination: VesselNomination
        BillOfLading: BillOfLading
    }


    and PhysicalDeliveryLeg =
    | VesselDeparted of VesselDepartedState // TODO: Add VesselSunk?
    | VesselArrived of VesselArrivedState  // TODO: Can it depart again before unloading?
    | InspectionPerformed of InspectionPerformedState
    | DischargeDocumentsCreated of DischargeDocumentsCreatedState
    

    and InTransitOrUnpaidState = {
        DepartureInfo: CaptainSigned<TerminalSigned<VesselDepartureInfo>>
        Physical: PhysicalDeliveryLeg
        Financial: FinancialLeg 
    }
    with 
        member __.``Progress Physical`` (handler: PhysicalDeliveryLeg -> PhysicalDeliveryLeg option) = 
            __.Physical 
            |> handler 
            |> Option.map (fun newLegState -> 
                            match newLegState, __.Financial with
                            | DischargeDocumentsCreated dc, PaymentMade pm -> DeliveredAndPaid(__.DepartureInfo, dc, pm)  // TODO: Add consistency check
                            | _ -> { __ with Physical = newLegState } |> InTransitOrUnpaid )

        member __.``Progress Financial`` (handler: FinancialLeg -> FinancialLeg option) = 
            __.Financial 
            |> handler
            |> Option.map (fun newLegState -> 
                            match __.Physical, newLegState with
                            | DischargeDocumentsCreated dc, PaymentMade pm -> DeliveredAndPaid(__.DepartureInfo, dc, pm)  // TODO: Add consistency check
                            | _ -> { __ with Financial = newLegState } |> InTransitOrUnpaid )
            

    and InvoiceData = {
        DepartureInfo: TerminalSigned<VesselDepartureInfo> // TODO: Fill-in the invoice with correct data 
        Date: DateTime
    }
    and SettlementInfo = {
        PayDate: DateTime
        SettlementDate: DateTime
    }

    and VesselDepartedState = {
        DepartureInfo: CaptainSigned<TerminalSigned<VesselDepartureInfo>>
        Progress: CaptainSigned<DeliveryProgress> option
    } 
    with 
        member __.``Notify of Progress`` progress = 
            { __ with Progress = Some progress } |> VesselDeparted
        member __.Arrive arrivalInfo = 
            {   DepartureInfo = __.DepartureInfo
                ArrivalInfo = arrivalInfo } |> VesselArrived        
    and DeliveryProgress = {
        Progress: float<percent> // TODO: Get proper details on this
    }
    and ArrivalInfo = {
        Date: DateTime // TODO: Get proper details on this
    }


    and VesselArrivedState = {
        DepartureInfo: CaptainSigned<TerminalSigned<VesselDepartureInfo>>
        ArrivalInfo: CaptainSigned<ArrivalInfo>
    } 
    with 
        member __.Inspection (inspectionResult: InspectorSigned<InspectionResult>) = 
            {   DepartureInfo = __.DepartureInfo
                InspectionResult = inspectionResult } |> InspectionPerformed


    and InspectionPerformedState = {
        DepartureInfo: CaptainSigned<TerminalSigned<VesselDepartureInfo>>
        InspectionResult: InspectorSigned<InspectionResult>
    } 
    with 
        member __.``Release Discharge Documents`` (dischargeDocuments: TerminalSigned<DischargeDocuments>) = 
            let offer = __.DepartureInfo.V.V.Nomination.CounterSignedDeal.V
            { DischargeDocuments = dischargeDocuments } |> DischargeDocumentsCreated


    and DischargeDocumentsCreatedState = {
        DischargeDocuments: TerminalSigned<DischargeDocuments>
    } 

    and FinancialLeg = 
    | ReadyForInvoicing of ReadyForInvoicingState
    | InvoiceCreated of InvoiceCreatedState // TODO: Separate into a new financial leg?
    | PaymentMade of PaymentMadeState

    and ReadyForInvoicingState = {
        InvoiceData: InvoiceData
    } 
    with 
        member __.``Send Invoice`` invoiceData = 
            { InvoiceCreatedState.InvoiceData = invoiceData } |> InvoiceCreated


    and InvoiceCreatedState = {
        InvoiceData: SellerSigned<InvoiceData>
        //InvoiceDate: DateTime
    } 
    with 
        member __.``Pay Invoice`` (settlementInfo: BuyerSigned<SettlementInfo>) = 
            
            { InvoiceData = __.InvoiceData; SettlementInfo = settlementInfo } |> PaymentMade

    and PaymentMadeState = {
        InvoiceData: SellerSigned<InvoiceData>
        SettlementInfo: BuyerSigned<SettlementInfo>
    } 

    and ArchivedState = CaptainSigned<TerminalSigned<VesselDepartureInfo>> * DischargeDocumentsCreatedState * PaymentMadeState

    type SellerContracts = Map<uint32, SellerContract>

    let updateContract (setContract: uint32 -> SellerContract -> unit) (contracts: unit -> SellerContracts) (msg: TradingMsg) (f: SellerContractState -> (SellerContractState) option) uid =
        let oldState = contracts().[uid]
        match oldState.State |> f with
        | Some state -> 
            let newState = oldState.Audit msg state
            setContract uid newState
        | None -> printfn "Unexpected contract state '%A'" oldState.State
        

    let allContracts (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)

    let sellerToAction uid (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | Created c -> c.Deal.V.Seller.Uid = uid
                                    | BuyerAccepted c -> c.CounterSignedDeal.V.V.Seller.Uid = uid
                                    | BuyerRejected c -> (c.RejectedDeal.V |> fst).V.Seller.Uid = uid
                                    | VesselOffered c -> c.CounterSignedDeal.V.V.Seller.Uid = uid
                                    | VesselNominationRejected c -> (c.RejectedNomination.V |> fst).CounterSignedDeal.V.V.Seller.Uid = uid
                                    | InTransitOrUnpaid state -> match state.Financial with 
                                                                    | ReadyForInvoicing c -> state.DepartureInfo.V.V.Nomination.CounterSignedDeal.V.V.Seller.Uid = uid
                                                                    | InvoiceCreated _
                                                                    | PaymentMade _ -> false
                                    | SentForAgreement _
                                    | VesselRequested _
                                    | VesselNominationAccepted _
                                    | VesselNominated _
                                    | LoadBoLCreated _
                                    | NORReleased _ 
                                    | DeliveredAndPaid _ -> false                    
                    )

    let buyerToAction uid (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | SentForAgreement c -> c.Deal.V.Buyer.Uid = uid
                                    | VesselNominated c -> c.Nomination.CounterSignedDeal.V.V.Buyer.Uid = uid
                                    | InTransitOrUnpaid state -> match state.Financial with 
                                                                    | InvoiceCreated c -> state.DepartureInfo.V.V.Nomination.CounterSignedDeal.V.V.Buyer.Uid = uid
                                                                    | ReadyForInvoicing _
                                                                    | PaymentMade _ -> false
                                    | Created _
                                    | BuyerRejected _
                                    | BuyerAccepted _ 
                                    | VesselRequested _
                                    | VesselOffered _
                                    | VesselNominationAccepted _
                                    | VesselNominationRejected _
                                    | NORReleased _
                                    | LoadBoLCreated _
                                    | DeliveredAndPaid _ -> false
                    )

    let traderToAction uid (contracts: unit -> SellerContracts) =
        join (buyerToAction uid contracts) (sellerToAction uid contracts)

    let vesselOperatorToAction uid (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | VesselRequested _
                                    | VesselOffered _ -> true
                                    | _ -> false
                    )

    let inspectorToAction uid (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | InTransitOrUnpaid state -> match state.Physical with 
                                                                    | VesselArrived c -> state.DepartureInfo.V.V.Nomination.CounterSignedDeal.V.V.Inspector.Uid = uid
                                                                    | _ -> false 
                                    | _ -> false
                    )

    let terminalLoadToAction uid (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | NORReleased c -> c.Nomination.CounterSignedDeal.V.V.TerminalLoad.Uid = uid
                                    | Created _
                                    | SentForAgreement _
                                    | BuyerRejected _
                                    | BuyerAccepted _ 
                                    | VesselRequested _
                                    | VesselOffered _
                                    | VesselNominated _
                                    | VesselNominationAccepted _
                                    | VesselNominationRejected _
                                    | LoadBoLCreated _
                                    | InTransitOrUnpaid _ 
                                    | DeliveredAndPaid _ -> false
                    )

    let terminalDischargeToAction uid (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | InTransitOrUnpaid state -> match state.Physical with 
                                                                    | InspectionPerformed c -> state.DepartureInfo.V.V.Nomination.CounterSignedDeal.V.V.TerminalDischarge.Uid = uid
                                                                    | _ -> false
                                    | _ -> false
                    )

    let terminalToAction uid (contracts: unit -> SellerContracts) =
        join (terminalLoadToAction uid contracts) (terminalDischargeToAction uid contracts)


    let captainToAction uid (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | VesselNominationAccepted c -> c.AcceptedNomination.V.NominatedVessel.V.V.Vessel.Captain.Uid = uid
                                    | LoadBoLCreated c -> c.DepartureInfo.V.Nomination.NominatedVessel.V.V.Vessel.Captain.Uid = uid
                                    | InTransitOrUnpaid state -> match state.Physical with 
                                                                    | VesselDeparted c -> c.DepartureInfo.V.V.Nomination.NominatedVessel.V.V.Vessel.Captain.Uid = uid
                                                                    | _ -> false
                                    | _ -> false
                    )


    let completedContracts (contracts: unit -> SellerContracts) =
        contracts()
        |> Map.map (fun k v -> v.State)
        |> Map.filter (fun _ c -> match c with 
                                    | DeliveredAndPaid _ -> true
                                    | _ -> false
                    )
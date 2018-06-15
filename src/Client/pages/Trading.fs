module Client.Trading

open System
open System.Text

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
open ServerCode.EntityList
open ServerCode.Utils

open Utils
open Entity
open Style
open Messages

open Client.Forms
open Client.Forms.CustomControls

module O = FSharp.Core.Option


/////////////////////

type Model = {
    Contracts: Map<UID, SellerContract>
}

let getContracts (model: Model)() = 
    model.Contracts |> Seq.map(fun kv -> kv.Key.ToString() |> JS.parseFloat |> uint32, kv.Value) |> Map.ofSeq
let setContract (model: Model) uid contract =
    printfn "UID: %d" uid
    { model with Contracts = model.Contracts.Add(uid, contract) } 
let removeContract (model: Model) uid =
    printfn "Removed UID: %d" uid
    if model.Contracts |> Map.containsKey uid then { model with Contracts = model.Contracts.Remove(uid) } 
    else model

///// ----- ToActions

let allContracts (model: Model) = getContracts model |> allContracts
let traderToDo uid (model: Model) = getContracts model |> traderToAction uid
let vesselOperatorToDo uid (model: Model) = getContracts model |> vesselOperatorToAction uid
let captainToDo uid (model: Model) = getContracts model |> captainToAction uid
let inspectorToDo uid (model: Model) = getContracts model |> inspectorToAction uid
let terminalToDo uid (model: Model) = getContracts model |> terminalToAction uid
let completedToDo (model: Model) = getContracts model |> completedContracts


let checkParticipation (roleChecker: SignerRole -> bool) uid (model: Model) = 
    let ret = (getContracts model)() 
                |> Map.filter (
                    fun k v -> v.History 
                                |> List.exists(fun hr -> match hr.Msg with 
                                                            | Some msg -> roleChecker(msg.Signature.Role) && msg.Signature.SignerUID = uid
                                                            | None -> false))
                |> Map.map(fun k v -> v.State)
    Browser.console.log ret
    ret

let traderParticipation = checkParticipation (fun role -> role = SignerRole.Seller || role = SignerRole.Buyer)

let vesselOperatorParticipation = checkParticipation (fun role -> role = SignerRole.VesselOperator)
let captainParticipation = checkParticipation (fun role -> role = SignerRole.Captain)
let inspectorParticipation = checkParticipation (fun role -> role = SignerRole.Inspector)
let terminalParticipation = checkParticipation (fun role -> role = SignerRole.TerminalLoad || role = SignerRole.TerminalDischarge)


let sign<'T, 'S> (signer: 'S) (data: 'T) = 
    let elliptic = importDefault("elliptic")?ec 
    let ec = createNew elliptic "secp256k1"
    let key = ec?genKeyPair()

    //let sha256 = importMember("sha.js")
    data |> toJson |> fun json -> 
        //let hasher = createNew sha256 () 
        let hasher = createNew (ec?hash) () 
        //hasher?update(json)?digest("hex").ToString() |> Sig 
        let strToBytes (str: string) = str |> Seq.map byte |> Seq.toArray
        let bytes = strToBytes json
        let signed = key?sign(bytes)
        let der: string = !!signed?toDER("hex")
        der |> Sig

let sellerSign (signer: Seller) (whatToSign: 'T) = 
    {   V = whatToSign
        Sig = { Role = SignerRole.Seller; SellerSignature.Signer = signer.Name; SignerUID = signer.Uid; Sig = sign signer whatToSign } }
let buyerSign (signer: Buyer) (whatToSign: 'T) = 
    {   V = whatToSign
        Sig = { Role = SignerRole.Buyer; BuyerSignature.Signer = signer.Name; SignerUID = signer.Uid; Sig = sign signer whatToSign } }
let vesselOperatorSign (signer: VesselOperator) (whatToSign: 'T) = 
    {   V = whatToSign
        Sig = { Role = SignerRole.VesselOperator; VesselOperatorSignature.Signer = signer.Name; SignerUID = signer.Uid; Sig = sign signer whatToSign } }
let inspectorSign (signer: Inspector) (whatToSign: 'T) = 
    {   V = whatToSign
        Sig = { Role = SignerRole.Inspector; InspectorSignature.Signer = signer.Name; SignerUID = signer.Uid; Sig = sign signer whatToSign } }
let terminalLoadSign (signer: Terminal) (whatToSign: 'T) = 
    {   V = whatToSign
        Sig = { Role = SignerRole.TerminalLoad; TerminalSignature.Signer = signer.Name; SignerUID = signer.Uid; Sig = sign signer whatToSign } }
let terminalDischargeSign (signer: Terminal) (whatToSign: 'T) = 
    {   V = whatToSign
        Sig = { Role = SignerRole.TerminalDischarge; TerminalSignature.Signer = signer.Name; SignerUID = signer.Uid; Sig = sign signer whatToSign } }
let captainSign (signer: Captain) (whatToSign: 'T) = 
    {   V = whatToSign
        Sig = { Role = SignerRole.Captain; CaptainSignature.Signer = signer.Name; SignerUID = signer.Uid; Sig = sign signer whatToSign } }

type PublicKey = string
type PrivateKey = string

type RegistryEntry = {
    Uid: Guid
    PubKeys: PublicKey list
    PrivKeys: PrivateKey list
}

// type BuyerRegistry = {

// }


///// ----- Show states
let states(model: Model) = (getContracts model)() 
                            |> Seq.map (fun kv -> (kv.Key, kv.Value.State)) 
                            |> Seq.sortBy fst

type BuySideTraderInfo(buyer: Buyer, potentialSellers: Buyer -> Seller list) = 
    member __.Buyer = buyer
    member __.PotentialSellers = fun () -> potentialSellers __.Buyer
    member __.Signer = buyerSign __.Buyer

type SellSideTraderInfo(seller: Seller, potentialBuyers: Seller -> Buyer list) = 
    member __.Seller = seller
    member __.PotentialBuyers = fun () -> potentialBuyers __.Seller
    member __.Signer whatToSign = sellerSign __.Seller whatToSign

type TraderInfo = {
    BuySide: BuySideTraderInfo
    SellSide: SellSideTraderInfo
}



type VesselOperatorInfo(vesselOperator: VesselOperator) = 
    member __.VesselOperator = vesselOperator
    member __.Signer whatToSign = vesselOperatorSign __.VesselOperator whatToSign

type TerminalLoadInfo(terminal: Terminal, potentialDestinations: Terminal -> Terminal list) = 
    member __.Terminal = terminal
    member __.PotentialDestinations = fun () -> potentialDestinations __.Terminal
    member __.Signer whatToSign = terminalLoadSign __.Terminal whatToSign

type TerminalDischargeInfo(terminal: Terminal) = 
    member __.Terminal = terminal
    member __.Signer whatToSign = terminalLoadSign __.Terminal whatToSign

type CompanyInfo = {
    UID: UID
    Name: string
}

type Page =
    | Trader of CompanyInfo
    | VesselOperator of CompanyInfo
    | VesselMaster of CompanyInfo
    | Terminal of CompanyInfo
    | Inspector of CompanyInfo
    | Archive 
    | All

let init () = //(user: UserData) (sp: Page) = 
    { Contracts = Map.empty }, Cmd.none 

let addNewContract model (contract: SellerContract) = 
    let newId = match contract.State with | Created c -> c.UID | _ -> 10000u
    setContract model newId contract

let defaultDeal = (sellerSign Examples.seller Examples.contractTerms)

let validateSignature signed = ()

let updateContract (msg: TradingMsg) model uid (f: SellerContractState -> SellerContractState option) = 
    let uid = uid.ToString() |> JS.parseFloat |> uint32 
    let mutable newModel = model
    Commodities.updateContract (fun _ sc -> 
                                    let m = setContract model uid sc 
                                    newModel <- m) (getContracts model) msg f uid
    newModel

let progressPhysicalLeg msg model uid (handler: PhysicalDeliveryLeg -> PhysicalDeliveryLeg option) = 
    updateContract msg model uid <| function
                                    | InTransitOrUnpaid cs -> handler |> cs.``Progress Physical``
                                    | _ -> None

let progressFinancialLeg msg model uid (handler: FinancialLeg -> FinancialLeg option) = 
    updateContract msg model uid <| function
                                | InTransitOrUnpaid cs -> handler |> cs.``Progress Financial``
                                | _ -> None

let resetDeal uid model : Model = removeContract model uid

let update (msg: TradingMsg) model : Model * Cmd<TradingMsg> = 
    Browser.console.log "TradingMsg received: "
    Browser.console.log msg
    let model = 
        match msg with
        | SellerMsg m -> 
            match m with 
            | AddContract (uid, deal) -> 
                let contract = addNewContract model (SellerContract.New uid deal)
                Browser.console.log "Contract created: "
                Browser.console.log contract
                contract
            | SendForAgreement (uid, deal)-> 
                validateSignature deal
                updateContract msg model uid 
                    <| function
                        | Created cs -> cs.``Send For Agreement``() |> Some
                        | _ -> None
            | RequestVessel (uid, svr) ->
                validateSignature svr
                updateContract msg model uid 
                    <| function
                        | BuyerAccepted cs -> cs.``Request Vessel`` svr |> Some
                        | _ -> None
            | NominateVessel (uid, vesselOffer) -> 
                validateSignature vesselOffer
                updateContract msg model uid 
                    <| function
                        | VesselOffered cs -> cs.``Nominate Vessel`` vesselOffer |> Some
                        | _ -> None
            | NominateAnotherVessel (uid, vesselOffer) -> 
                validateSignature vesselOffer
                updateContract msg model uid 
                    <| function
                        | VesselNominationRejected cs -> cs.``Nominate Another Vessel`` (cs.RejectedNomination.V |> fst).NominatedVessel |> Some 
                        | _ -> None
            | RequestAnotherVessel (uid, svr) ->
                validateSignature svr
                updateContract msg model uid 
                    <| function
                        | VesselNominationRejected cs -> cs.``Request Another Vessel`` svr |> Some 
                        | _ -> None
            | ResendForAgreement (uid, deal) -> 
                validateSignature deal
                updateContract msg model uid 
                    <| function
                        | BuyerRejected cs -> cs.``Resend For Agreement`` deal |> Some 
                        | VesselNominationRejected cs -> cs.``Resend For Agreement`` deal |> Some 
                        | _ -> None
            | IssueInvoice (uid, invoiceData) ->
                validateSignature invoiceData
                progressFinancialLeg msg model uid 
                    <| function
                        | ReadyForInvoicing fcs -> fcs.``Send Invoice`` invoiceData |> Some
                        | _ -> None
        | BuyerMsg m -> 
            match m with
            | BuyerAccept (uid, csd) ->
                validateSignature csd
                updateContract msg model uid 
                    <| function
                        | SentForAgreement cs -> cs.Accept csd |> Some
                        | _ -> None
            | BuyerReject (uid, sbr) ->
                validateSignature sbr
                updateContract msg model uid 
                    <| function
                        | SentForAgreement cs -> cs.Reject sbr |> Some
                        | _ -> None
            | AcceptNomination (uid, nv) ->
                validateSignature nv 
                updateContract msg model uid 
                    <| function
                        | VesselNominated cs -> cs.``Accept Nomination`` nv |> Some
                        | _ -> None
            | RejectionNomination (uid, rvn) -> 
                validateSignature rvn
                updateContract msg model uid 
                    <| function
                        | VesselNominated cs -> cs.``Reject Nomination`` rvn |> Some
                        | _ -> None
            | PayInvoice (uid, ssi) -> 
                validateSignature ssi  
                progressFinancialLeg msg model uid 
                    <| function
                        | InvoiceCreated fcs -> fcs.``Pay Invoice`` ssi |> Some
                        | _ -> None
        | VesselOperatorMsg m ->
            match m with
            | OfferVessel (uid, vesselOffer) ->
                validateSignature vesselOffer
                updateContract msg model uid 
                    <| function
                        | VesselRequested cs -> cs.``Offer a Vessel`` vesselOffer |> Some
                        | VesselOffered cs -> cs.``Offer a Vessel`` vesselOffer |> Some
                        | _ -> None
        | TerminalLoadMsg m -> 
            match m with
            | CreateLoadBoL (uid, vdi) -> 
                validateSignature vdi
                updateContract msg model uid
                    <| function
                        | NORReleased cs -> cs.CreateLoadBoL vdi |> Some 
                        | _ -> None
        | TerminalDischargeMsg m -> 
            match m with
            | ReleaseDischargeDocuments (uid, dischargeDocuments) -> 
                validateSignature dischargeDocuments
                progressPhysicalLeg msg model uid 
                    <| function
                        | InspectionPerformed pcs -> pcs.``Release Discharge Documents`` dischargeDocuments |> Some 
                        | _ -> None
        | InspectorMsg m ->
            match m with
            | PerformInspection (uid, inspectionResults) -> 
                validateSignature inspectionResults
                progressPhysicalLeg msg model uid 
                    <| function
                        | VesselArrived pcs -> pcs.Inspection inspectionResults |> Some
                        | _ -> None
        | CaptainMsg m -> 
            match m with
            | ReleaseNOR (uid, nor) ->
                validateSignature nor
                updateContract msg model uid 
                    <| function
                        | VesselNominationAccepted cs -> cs.``Notice of Readiness`` nor |> Some
                        | _ -> None
            | DepartVessel (uid, departureInfo) ->
                validateSignature departureInfo
                updateContract msg model uid 
                    <| function
                        | LoadBoLCreated cs -> cs.``Depart Vessel`` departureInfo |> Some
                        | _ -> None
            | NotifyOfVesselProgress (uid, deliveryProgress) -> 
                validateSignature deliveryProgress
                progressPhysicalLeg msg model uid 
                    <| function
                        | VesselDeparted pcs -> pcs.``Notify of Progress`` deliveryProgress |> Some
                        | _ -> None
            | ArriveVessel (uid, arrivalInfo) -> 
                validateSignature arrivalInfo
                progressPhysicalLeg msg model uid 
                    <| function
                        | VesselDeparted pcs -> pcs.Arrive arrivalInfo |> Some
                        | _ -> None
    model, Cmd.none

open Fable.Import.BootstrapTable

type TableViewModel = {
    uid: UID
    value: SellerContractState
}

let actionButton subsclass txt (dispatch: unit -> unit) =
    button [ClassName ("btn btn-sm " + subsclass) 
            OnClick (fun _ -> dispatch ())
            ] [ text txt ]

let sellerActionButtons seller dispatch (row: TableViewModel) = 
    let dispatch = SellerMsg >> dispatch
    match (row.value) with 
    | Created state -> [ actionButton "btn-success" "Send for Agreement" (fun () -> SendForAgreement (row.uid, state.Deal) |> dispatch) ]
    | BuyerAccepted state -> [ actionButton "btn-success" "Request a Vessel" (fun () -> 
                                                                                let ct = state.CounterSignedDeal.V.V
                                                                                RequestVessel (row.uid, {   VesselRequest.Capacity = ct.Quantity
                                                                                                            Terminal = ct.TerminalLoad
                                                                                                            Destination = ct.TerminalDischarge
                                                                                                            DepartureDate = ct.DateLoad
                                                                                                            ArrivalDate = ct.DateDischarge
                                                                                                        } |> sellerSign seller) |> dispatch) ]
    | BuyerRejected state -> 
        let deal = state.RejectedDeal.V |> fst
        [   com<CreateNewContractForm, _, _>( { Title = "Resend for Agreement"
                                                ContractTerms = deal.V
                                                Buyers = [| Examples.buyer |]
                                                Inspectors = [| Examples.inspector; Examples.inspector2 |]
                                                LoadTerminals = Examples.terminals |> List.toArray
                                                DischargeTerminals = Examples.terminals |> List.toArray
                                                Signer = fun ct -> sellerSign seller ct
                                                Dispatch = (fun d -> ResendForAgreement (row.uid, d)) >> dispatch
                                            }) []
        ]
    | VesselOffered state -> [  com<VesselNominateForm, _, _>({ ButtonCaption = "Nominate Vessel"
                                                                Offers = state.Offers
                                                                Signer = sellerSign seller
                                                                Dispatch = fun snv -> (row.uid, snv) |> NominateVessel |> dispatch }) []
                            ]
    | VesselNominationRejected state -> 
        [   com<VesselNominateForm, _, _>({ ButtonCaption = "Nominate Another Vessel"
                                            Offers = state.VesselOffers
                                            Signer = sellerSign seller
                                            Dispatch = fun snv -> (row.uid, snv) |> NominateAnotherVessel |> dispatch }) []
            actionButton "btn-success" "Request Another Vessel" (
                fun () -> 
                    let ct = (state.RejectedNomination.V |> fst).CounterSignedDeal.V.V
                    RequestAnotherVessel (row.uid, {VesselRequest.Capacity = ct.Quantity
                                                    Terminal = ct.TerminalLoad
                                                    Destination = ct.TerminalDischarge
                                                    DepartureDate = ct.DateLoad
                                                    ArrivalDate = ct.DateDischarge
                                                } |> sellerSign seller) |> dispatch) 
            actionButton "btn-success" "Resend for Agreement" (fun () -> ResendForAgreement (row.uid, defaultDeal) |> dispatch) 
                                    ]
    | InTransitOrUnpaid st -> match st.Financial with 
                                | ReadyForInvoicing state -> 
                                    [ actionButton "btn-success" "Issue an Invoice" (
                                        fun () -> IssueInvoice (row.uid, state.InvoiceData |> sellerSign seller) |> dispatch) ]
                                | _ -> []
    | _ -> []

let buyerActionButtons buyer dispatch (row: TableViewModel) = 
    let dispatch = BuyerMsg >> dispatch
    match (row.value) with
    | SentForAgreement state -> [   actionButton "btn-success" "Accept" (fun () -> (row.uid, state.Deal |> buyerSign buyer) |> BuyerAccept |> dispatch)
                                    com<RejectionForm, _, _>({ Deal = state.Deal; Signer = buyerSign buyer; Dispatch = fun srd -> (row.uid, srd) |> BuyerReject |> dispatch }) []
                                ]
    | VesselNominated state -> [    actionButton "btn-success" "Accept Nomination" (fun () -> (row.uid, state.Nomination |> buyerSign buyer) |> AcceptNomination |> dispatch)
                                    com<RejectNominationForm, _, _>({ VesselNomination = state.Nomination; Signer = buyerSign buyer; Dispatch = fun srd -> (row.uid, srd) |> RejectionNomination |> dispatch }) []
                                ]
    | InTransitOrUnpaid state -> 
        match state.Financial with 
        | InvoiceCreated _ -> 
            [ actionButton "btn-success" "Pay Invoice" (fun () -> PayInvoice (row.uid, { PayDate = (DateTime.Today.AddDays 5.); SettlementDate = (DateTime.Today.AddDays 8.) } |> buyerSign buyer) |> dispatch) ]
        | _ -> []
    | _ -> [] 
    
let traderActionButtons (trader: Trader) dispatch (row: TableViewModel) = 
    (sellerActionButtons trader.Seller dispatch row) @ (buyerActionButtons trader.Buyer dispatch row)

let vesselOperatorActionButtons vesselOperator dispatch (row: TableViewModel) = 
    let dispatch = VesselOperatorMsg >> dispatch
    match (row.value) with
    | VesselRequested state -> [com<VesselOfferForm, _, _>({VesselRequest = state.Request
                                                            Fleet = (Examples.operatorFleet |> List.find (fun vof -> vof.Operator = vesselOperator)).Fleet
                                                            Signer = vesselOperatorSign vesselOperator
                                                            Dispatch = fun svo -> (row.uid, svo) |> OfferVessel |> dispatch }) []
                            ] 
    | VesselOffered state -> [  com<VesselOfferForm, _, _>({VesselRequest = state.Request
                                                            Fleet = (Examples.operatorFleet |> List.find (fun vof -> vof.Operator = vesselOperator)).Fleet
                                                            Signer = vesselOperatorSign vesselOperator
                                                            Dispatch = fun svo -> (row.uid, svo) |> OfferVessel |> dispatch }) []                            
                            ]
    | _ -> []

let terminalLoadActionButtons terminalLoad dispatch (row: TableViewModel) = 
    let dispatch = TerminalLoadMsg >> dispatch
    match (row.value) with
    | NORReleased state -> [ com<TerminalDocumentsForm, _, _>({ Nomination = state.Nomination
                                                                BillOfLading = None
                                                                Dispatch = fun bol -> (row.uid, {   VesselDepartureInfo.Nomination = state.Nomination
                                                                                                    BillOfLading = bol } |> terminalLoadSign terminalLoad) |> CreateLoadBoL |> dispatch }) []
                        ]
    | _ -> []

let terminalDischargeActionButtons terminalDischarge dispatch (row: TableViewModel) = 
    let dispatch = TerminalDischargeMsg >> dispatch
    match (row.value) with
    | InTransitOrUnpaid state -> 
        match state.Physical with 
        | InspectionPerformed state -> [com<TerminalDocumentsForm, _, _>({  Nomination = state.DepartureInfo.V.V.Nomination
                                                                            BillOfLading = state.DepartureInfo.V.V.BillOfLading |> Some
                                                                            Dispatch = fun bol -> (row.uid, { BillOfLading = bol } |> terminalDischargeSign terminalDischarge) |> ReleaseDischargeDocuments |> dispatch }) [] 
                                        ]
        | _ -> []
    | _ -> []

let terminalActionButtons (terminal: Terminal) dispatch (row: TableViewModel) = 
    (terminalLoadActionButtons terminal dispatch row) @ (terminalDischargeActionButtons terminal dispatch row)

let inspectorActionButtons inspector dispatch (row: TableViewModel) =
    let dispatch = InspectorMsg >> dispatch
    match (row.value) with
    | InTransitOrUnpaid state -> 
        match state.Physical with 
        | VesselArrived state -> [  com<InspectionForm, _, _>({ DepartureInfo = state.DepartureInfo
                                                                ArrivalInfo = state.ArrivalInfo
                                                                Signer = inspectorSign inspector
                                                                Dispatch = fun svo -> (row.uid, svo) |> PerformInspection |> dispatch }) [] 
                                ]
        | _ -> [] 
    | _ -> []

let captainActionButtons captain dispatch (row: TableViewModel) = 
    let dispatch = CaptainMsg >> dispatch
    match (row.value) with
    | VesselNominationAccepted state -> [ actionButton "btn-success" "Release Notice of Readiness" (
                                            fun () -> 
                                                let nomination = state.AcceptedNomination.V
                                                let contractTerms = nomination.CounterSignedDeal.V.V
                                                let nor = { Terminal = contractTerms.TerminalLoad
                                                            Date = contractTerms.DateLoad
                                                            Vessel = nomination.NominatedVessel.V.V |> fun vo -> vo.Vessel 
                                                            Quantity = contractTerms.Quantity }
                                                (row.uid, nor |> captainSign captain) |> ReleaseNOR |> dispatch ) ]
    | LoadBoLCreated state -> [ actionButton "btn-success" "Depart Vessel" (fun () -> (row.uid, state.DepartureInfo |> captainSign captain) |> DepartVessel |> dispatch) ]
    | InTransitOrUnpaid state -> 
        match state.Physical with 
        | VesselDeparted st -> [    com<VesselProgressForm, _, _>({ DepartureInfo = st.DepartureInfo
                                                                    Progress = st.Progress
                                                                    Signer = captainSign captain
                                                                    Dispatch = fun srd -> (row.uid, srd) |> NotifyOfVesselProgress |> dispatch }) []
                                    actionButton "btn-success" "Arrive Vessel" (fun () -> (row.uid, { Date = DateTime.Today } |> captainSign captain) |> ArriveVessel |> dispatch) ]
        | _ -> []
    | _ -> []


let statusColumn =
    TableHeaderColumn 
        (fun p ->   p.dataFormat <- Some (Func<_,_,_,_>
                        (fun cell row enumObject -> 
                            let row = row :?> TableViewModel
                            row.value.StateName |> U2.Case1))
                    p.dataSort <- Some true) [ text "Status" ]

let contractColumn =
    TableHeaderColumn 
        (fun p ->   p.dataFormat <- Some (Func<_,_,_,_>
                        (fun cell row enumObject -> 
                            let row = row :?> TableViewModel
                            row.value |> contractStateControl |> U2.Case2))
                    p.dataSort <- Some true) [ text "Contract" ]

let availableActionsColumn (dispatch: TradingMsg -> unit) buttons =
    TableHeaderColumn 
        (fun p ->   p.dataFormat <- Some (Func<_,_,_,_>
                        (fun cell row enumObject -> div [] (buttons dispatch (row :?> TableViewModel)) |> U2.Case2))
                    p.dataSort <- Some true
                    p.width <- Some "250") [ text "Available Actions" ]
                    

let contractTable model dispatch (todo: Model -> Map<UID, SellerContractState>) actionButtons =
    BootstrapTable (fun p ->p.data <- ResizeArray(model|> todo |> Seq.map (fun kv -> { uid = kv.Key; value = kv.Value} :> obj)) 
                            p.pagination <- Some true 
                            p.options <- Some (TableOptions(fun o -> o.hideSizePerPage <- Some true))) [
        TableHeaderColumn (fun p -> p.dataField <- Some "uid"
                                    p.dataSort <- Some true
                                    p.isKey <- Some true
                                    p.width <- Some "90") [ text "Id" ] 
        contractColumn
        availableActionsColumn dispatch actionButtons
    ]

let contractParticipationTable model dispatch (todo: Model -> Map<UID, SellerContractState>) =
    BootstrapTable (fun p ->p.data <- ResizeArray(model|> todo |> Seq.map (fun kv -> { uid = kv.Key; value = kv.Value} :> obj)) 
                            p.pagination <- Some true 
                            p.options <- Some (TableOptions(fun o -> o.hideSizePerPage <- Some true))) [
        TableHeaderColumn (fun p -> p.dataField <- Some "uid"
                                    p.dataSort <- Some true
                                    p.isKey <- Some true
                                    p.width <- Some "90") [ text "Id" ] 
        contractColumn
    ]

let private rnd = Random()

let innerView (page: Page) (model: Model) (dispatch: TradingMsg -> unit) =
    match page with
    | Trader info ->
        let trader = Examples.traders |> List.find (fun t -> t.Uid = info.UID)
        let potentialBuyers = Examples.traders |> List.filter (fun t -> t.Buyer.Uid <> info.UID) |> List.map (fun t -> t.Buyer) |> List.toArray
        let defaultDeal = { defaultDeal.V with Seller = trader.Seller; Buyer = potentialBuyers.[0] } |> sellerSign trader.Seller
        div [ ClassName "row" ] [
            div [] [
                button [ClassName "btn btn-sm btn-info" 
                        OnClick (fun _ -> dispatch (AddContract (rnd.Next(10000) |> uint32, defaultDeal) |> SellerMsg) )
                        Style [ CSSProp.MarginRight "4px" ]
                    ] [ text "Create New Default Contract" ]
            ]
            com<CreateNewContractForm, _, _>(
                {   Title = "Create New Contract"
                    ContractTerms = defaultDeal.V
                    Buyers = potentialBuyers
                    Inspectors = Examples.inspectors |> List.toArray
                    LoadTerminals = Examples.terminals |> List.toArray
                    DischargeTerminals = Examples.terminals |> List.toArray
                    Signer = fun ct -> ct |> sellerSign trader.Seller
                    Dispatch = fun deal -> AddContract (rnd.Next(10000) |> uint32, deal) |> SellerMsg |> dispatch
                }) []
            hr []
            contractTable model dispatch (traderToDo info.UID) (traderActionButtons trader)
        ]
    | VesselOperator info -> contractTable model dispatch (vesselOperatorToDo info.UID) (vesselOperatorActionButtons (Examples.vesselOperators |> List.find (fun t -> t.Uid = info.UID)))
    | Terminal info -> contractTable model dispatch (terminalToDo info.UID) (terminalActionButtons (Examples.terminals |> List.find (fun t -> t.Uid = info.UID))) 
    | Inspector info -> contractTable model dispatch (inspectorToDo info.UID) (inspectorActionButtons (Examples.inspectors |> List.find (fun t -> t.Uid = info.UID)))
    | VesselMaster info -> contractTable model dispatch (captainToDo info.UID) (captainActionButtons (Examples.captains |> List.find (fun t -> t.Uid = info.UID)))
    | Archive -> contractTable model dispatch completedToDo (fun _ _ -> [])
    | All -> contractTable model dispatch allContracts (fun _ _ -> [])

let innerViewParticipation (page: Page) (model: Model) (dispatch: TradingMsg -> unit) =
    match page with
    | Trader info -> contractParticipationTable model dispatch (traderParticipation info.UID) 
    | VesselOperator info -> contractParticipationTable model dispatch (vesselOperatorParticipation info.UID) 
    | Terminal info -> contractParticipationTable model dispatch (terminalParticipation info.UID) 
    | Inspector info -> contractParticipationTable model dispatch (inspectorParticipation info.UID) 
    | VesselMaster info -> contractParticipationTable model dispatch (captainParticipation info.UID) 
    | Archive -> br []
    | All -> br []

let view (p: Page) (model: Model) (dispatch: TradingMsg -> unit) (loadingDispatch: bool -> unit) = 
    div [] [
        yield div [ ClassName "card"] [
            div [ ClassName "card-header"] [
                strong [] [(match p with
                            | All -> "All contracts "
                            | Archive -> "Archived contracts" 
                            | p -> p |> getUnionCaseNameSplit |> fun s -> s + " '" + !!p?data?Name + "' to-do list") |> unbox ]
            ]
            div [ ClassName "card-block"] [
                div [ ClassName "row"] [
                    div [ ClassName "col-sm-12"] [
                        innerView p model dispatch
                    ]
                ]
            ]
        ]
        match p with
        | All -> ()
        | Archive -> ()
        | _ -> 
            yield br [] //TODO: Fix the navigation buttons placement properly
            yield br []
            yield br []
            yield div [ ClassName "card"] [
                div [ ClassName "card-header"] [
                    strong [] [ p |> getUnionCaseNameSplit |> fun s -> s + " '" + !!p?data?Name + "' activity history" |> unbox ]
                ]
                div [ ClassName "card-block"] [
                    div [ ClassName "row"] [
                        div [ ClassName "col-sm-12"] [
                            innerViewParticipation p model dispatch
                        ]
                    ]
                ]
            ]
    ]
        
let pageDefs = 
    (Examples.traders |> List.map (fun t -> Trader { UID = t.Uid; Name = t.Name }, traderToDo t.Seller.Uid)) @
    (Examples.vesselOperators |> List.map (fun t -> VesselOperator { UID = t.Uid; Name = t.Name }, vesselOperatorToDo t.Uid)) @
    (Examples.captains |> List.map (fun t -> VesselMaster { UID = t.Uid; Name = t.Name }, captainToDo t.Uid)) @
    (Examples.terminals |> List.map (fun t -> Terminal { UID = t.Uid; Name = t.Name }, terminalToDo t.Uid)) @
    (Examples.inspectors |> List.map (fun t -> Inspector { UID = t.Uid; Name = t.Name }, inspectorToDo t.Uid)) @
    [   Archive, completedToDo
        All, allContracts
    ]

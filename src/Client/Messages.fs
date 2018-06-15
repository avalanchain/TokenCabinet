module Client.Messages

open System

open ServerCode
open ServerCode.Domain
open ServerCode.Commodities
open ServerCode.EntityList

open Client.Entity

/// The messages processed during login 
type LoginMsg =
  | GetTokenSuccess of string
  | SetUserName of string
  | SetPassword of string
  | AuthError of exn
  | ClickLogIn

type MenuMsg = | Logout  

type StaticsMsg = 
  | BookMsg of EntityListMsg<Book>
  | IndividualMsg of EntityListMsg<Individual>
  | OrganizationMsg of EntityListMsg<Organization>
  | VesselMsg of EntityListMsg<Vessel>
  | BillOfLadingMsg of EntityListMsg<BillOfLading>



type TestMsg = {
  Name: string
  // Capacity: Volume 
  // Operator: VesselOperator
  // IMO: uint32 option
  // MMSI: uint32
  // CallSign: string
  // Flag: string
  // VesselType: VesselType 
  // Built: uint16 option
}

type AdminMsg = | Reload

type EthConnection = {
    EthContractAddress: string
    EthContractAbi: obj
    EthContract: obj
    EthDispatcher: TradingMsg -> unit
    HistoryLoading: Set<UID>
}

type EthereumMsg = 
  | CreateEthContractMsg 
  | ConnectToEthContractMsg of contractAddr: string
  | ConnectedToEthContractMsg of EthConnection
  | LoadDealHistoryMsg of UID

/// The different messages processed by the application
type AppMsg = 
  | LoggedIn
  | LoggedOut
  | StorageFailure of exn
  | OpenLogIn
  | LoginMsg of LoginMsg
  | AdminMsg of AdminMsg
  | StaticsMsg of StaticsMsg
  | TradingMsg of TradingMsg
  | MenuMsg of MenuMsg
  | LoadingMsg of bool
  | EthereumMsg of EthereumMsg

  // | TestMsg of TestMsg

/// The different pages of the application. If you add a new page, then add an entry here.



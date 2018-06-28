module Client.CabinetPage

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
open LoginPage
open Shared
open Client.CabinetModel

type Page =
    | PurchaseToken
    | MyInvestments
    | ReferralProgram
    | Verification
    | Contacts
    | Dashboard
    with static member Default = PurchaseToken  

// type Model = 
//     | VerificationModel     of string
//     | PurchaseTokenModel    of ViewModels.TokenSale option
//     | MyInvestmentsModel    of string
//     | ReferralProgramModel  of string
//     | ContactsModel         of string
//     | DashboardModel        of string



type Msg =
    | VerificationMsg
    | PurchaseTokenMsg
    | MyInvestmentsMsg
    | ReferralProgramMsg
    | ContactsMsg
    | DashboardMsg
    | ServerMsg     of ServerMsg
and ServerMsg =
    | GetCryptoCurrenciesCompleted  of ViewModels.CryptoCurrency list
    | GetTokenSaleCompleted         of ViewModels.TokenSale
    | GetFullCustomerCompleted      of ViewModels.FullCustomer
    | PriceTick                     of ViewModels.CurrencyPriceTick

let init () = 
    {   CryptoCurrencies        = []
        CurrenciesCurentPrices  = { Prices = [] }
        TokenSale               = None
        FullCustomer            = None }
   

    // | Book ed -> 
    //     let m,cmd = EntityDefs.book.Init user
    //     m |> BookListModel, Cmd.map (BookMsg) cmd  
    // | Individual ed -> 
    //     let m,cmd = EntityDefs.individual.Init user
    //     m |> IndividualListModel, Cmd.map (IndividualMsg) cmd  
    // | Organization ed -> 
    //     let m,cmd = EntityDefs.organization.Init user
    //     m |> OrganizationListModel, Cmd.map (OrganizationMsg) cmd  
    // | Vessel ed -> 
    //     let m,cmd = EntityDefs.vessel.Init user
    //     m |> VesselListModel, Cmd.map (VesselMsg) cmd  
    // | BillOfLading ed -> 
    //     let m,cmd = EntityDefs.billOfLading.Init user
    //     m |> BillOfLadingListModel, Cmd.map (BillOfLadingMsg) cmd  


let update (msg: Msg) model : Model * Cmd<Msg> = //model ,Cmd.none
    match msg with
    | VerificationMsg    -> model, Cmd.none
    | PurchaseTokenMsg   -> model, Cmd.none
    | MyInvestmentsMsg   -> model, Cmd.none
    | ReferralProgramMsg -> model, Cmd.none
    | ContactsMsg        -> model, Cmd.none
    | DashboardMsg       -> model, Cmd.none
    | ServerMsg msg_     ->
        match msg_ with
        | GetCryptoCurrenciesCompleted cc   -> { model with CryptoCurrencies = cc } , Cmd.none
        | GetTokenSaleCompleted tc          -> { model with TokenSale = Some (tc) } , Cmd.none
        | GetFullCustomerCompleted fc       -> { model with FullCustomer = Some (fc) } , Cmd.none
        | PriceTick tick                    -> { model with CurrenciesCurentPrices = tick }, Cmd.none

    // | BookListModel m, BookMsg ms -> 
    //     let m, cmd = EntityDefs.book.Update ms m
    //     m |> BookListModel, Cmd.map (BookMsg) cmd


    // | (m, msg) -> 
    //     Browser.console.error(sprintf "Unexpected Model '%A' and Msg '%A' combination" m msg)
    //     model, Cmd.none

open ReactBootstrap
open Fable.Import.React
open Client.Helpers


let view (page: Page) (model: Model) (dispatch: Msg -> unit) = 
    match page with
        | Verification      -> [ VerificationPage.view ]
        | PurchaseToken     -> [ PurchaseTokenPage.view model]
        | MyInvestments     -> [ str "My Investments view" ]
        | ReferralProgram   -> [ str "Referral Program view" ]
        | Contacts               -> 
            [   str "Contacts view"
                comE buttonToolbar [
                    comE button [
                        str "Default"
                    ]
                    comF button (fun o -> o.bsStyle <- Some "primary" ) [
                        str "Default"
                    ]
                    comF button (fun o ->   o.bsStyle <- Some "warning"
                                            o.onClick <- Some (MouseEventHandler(fun e -> e.stopPropagation())) ) [
                        str "Default"
                    ]    
                ] 
            ]
        | Dashboard             -> [ DashboardView.view ]
        // | Book ed, BookListModel m -> viewPageStatic<Book> ed m dispatch
        // | Individual ed, IndividualListModel m -> viewPageStatic<Individual> ed m dispatch
        // | Organization ed, OrganizationListModel m -> viewPageStatic<Organization> ed m dispatch
        // | Vessel ed, VesselListModel m -> viewPageStatic<Vessel> ed m dispatch
        // | BillOfLading ed, BillOfLadingListModel m -> viewPageStatic<BillOfLading> ed m dispatch
        // | p -> 
        //     Browser.console.error(sprintf "Impossible Page:[%A] - Model:[%A] combination" p)
        //     [ ]
    |> div [] 
        




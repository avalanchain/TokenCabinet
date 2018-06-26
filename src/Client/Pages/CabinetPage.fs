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

type Page =
    | PurchaseToken
    | MyInvestments
    | ReferralProgram
    | Verification
    | Contacts
    | Dashboard
    with static member Default = PurchaseToken  

type Model = 
    | VerificationModel     of string
    | PurchaseTokenModel    of string
    | MyInvestmentsModel    of string
    | ReferralProgramModel  of string
    | ContactsModel         of string
    | DashboardModel        of string

type Msg =
    | VerificationMsg
    | PurchaseTokenMsg
    | MyInvestmentsMsg
    | ReferralProgramMsg
    | ContactsMsg
    | DashboardMsg

let init (user: AuthModel) (page: Page) = 
    match page with 
    | Verification      -> VerificationModel "VerificationModel",       Cmd.none
    | PurchaseToken     -> PurchaseTokenModel "PurchaseTokenModel",     Cmd.none
    | MyInvestments     -> MyInvestmentsModel "MyInvestmentsModel",     Cmd.none
    | ReferralProgram   -> ReferralProgramModel "ReferralProgramModel", Cmd.none
    | Contacts          -> ContactsModel "ContactsModel",               Cmd.none
    | Dashboard         -> DashboardModel "DashboardModel",             Cmd.none

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


let update (msg: Msg) model : Model * Cmd<Msg> = 
    match model, msg with
    | VerificationModel m,    VerificationMsg    -> VerificationModel "Verification update",   Cmd.none
    | PurchaseTokenModel m,   PurchaseTokenMsg   -> PurchaseTokenModel "PurchaseToken update", Cmd.none
    | MyInvestmentsModel m,   MyInvestmentsMsg   -> MyInvestmentsModel "MyInvestments update", Cmd.none
    | ReferralProgramModel m, ReferralProgramMsg -> ReferralProgramModel "ReferralProgramModel update", Cmd.none
    | ContactsModel m,        ContactsMsg        -> ContactsModel "ContactsModel update", Cmd.none
    | DashboardModel m,       DashboardMsg       -> DashboardModel "DashboardModel update", Cmd.none

    // | BookListModel m, BookMsg ms -> 
    //     let m, cmd = EntityDefs.book.Update ms m
    //     m |> BookListModel, Cmd.map (BookMsg) cmd
    // | IndividualListModel m, IndividualMsg ms -> 
    //     let m, cmd = EntityDefs.individual.Update ms m
    //     m |> IndividualListModel, Cmd.map (IndividualMsg) cmd
    // | OrganizationListModel m, OrganizationMsg ms -> 
    //     let m, cmd = EntityDefs.organization.Update ms m
    //     m |> OrganizationListModel, Cmd.map (OrganizationMsg) cmd
    // | VesselListModel m, VesselMsg ms -> 
    //     let m, cmd = EntityDefs.vessel.Update ms m
    //     m |> VesselListModel, Cmd.map (VesselMsg) cmd
    // | BillOfLadingListModel m, BillOfLadingMsg ms -> 
    //     let m, cmd = EntityDefs.billOfLading.Update ms m
    //     m |> BillOfLadingListModel, Cmd.map (BillOfLadingMsg) cmd
    | (m, msg) -> 
        Browser.console.error(sprintf "Unexpected Model '%A' and Msg '%A' combination" m msg)
        model, Cmd.none

open ReactBootstrap
open Fable.Import.React
open Client.Helpers


let view (page: Page) (model: Model) (dispatch: Msg -> unit) = 
    match page, model with
        | Verification, VerificationModel m       -> [ str "Verification view" ]
        | PurchaseToken, PurchaseTokenModel m     -> [ str "Purchase Token view" ]
        | MyInvestments, MyInvestmentsModel m     -> [ str "My Investments view" ]
        | ReferralProgram, ReferralProgramModel m -> [ str "Referral Program view" ]
        | Contacts, ContactsModel m               -> 
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
        | Dashboard, DashboardModel m             -> [ str "Dashboard view" ]
        // | Book ed, BookListModel m -> viewPageStatic<Book> ed m dispatch
        // | Individual ed, IndividualListModel m -> viewPageStatic<Individual> ed m dispatch
        // | Organization ed, OrganizationListModel m -> viewPageStatic<Organization> ed m dispatch
        // | Vessel ed, VesselListModel m -> viewPageStatic<Vessel> ed m dispatch
        // | BillOfLading ed, BillOfLadingListModel m -> viewPageStatic<BillOfLading> ed m dispatch
        | p, m -> 
            Browser.console.error(sprintf "Impossible Page:[%A] - Model:[%A] combination" p m)
            [ ]
    |> div [] 
        




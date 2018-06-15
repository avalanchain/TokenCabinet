module Client.Statics

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
open ServerCode.EntityList

open Entity
open Style
open Messages

type Model = 
  | BookListModel of EntityModel<Book>
  | IndividualListModel of EntityModel<Individual>
  | OrganizationListModel of EntityModel<Organization>
  | VesselListModel of EntityModel<Vessel>
  | BillOfLadingListModel of EntityModel<BillOfLading>

type Page =
  | Book of EntityDef<EntityList.Book, AppMsg>
  | Individual of EntityDef<Commodities.Individual, AppMsg>
  | Organization of EntityDef<Commodities.Organization, AppMsg>
  | Vessel of EntityDef<Commodities.Vessel, AppMsg>
  | BillOfLading of EntityDef<Commodities.BillOfLading, AppMsg>

let init (user: UserData) (sp: Page) = 
    match sp with 
    | Book ed -> 
        let m,cmd = EntityDefs.book.Init user
        m |> BookListModel, Cmd.map (BookMsg) cmd  
    | Individual ed -> 
        let m,cmd = EntityDefs.individual.Init user
        m |> IndividualListModel, Cmd.map (IndividualMsg) cmd  
    | Organization ed -> 
        let m,cmd = EntityDefs.organization.Init user
        m |> OrganizationListModel, Cmd.map (OrganizationMsg) cmd  
    | Vessel ed -> 
        let m,cmd = EntityDefs.vessel.Init user
        m |> VesselListModel, Cmd.map (VesselMsg) cmd  
    | BillOfLading ed -> 
        let m,cmd = EntityDefs.billOfLading.Init user
        m |> BillOfLadingListModel, Cmd.map (BillOfLadingMsg) cmd  


let update (msg: StaticsMsg) model : Model * Cmd<StaticsMsg> = 
    match model, msg with
    | BookListModel m, BookMsg ms -> 
        let m, cmd = EntityDefs.book.Update ms m
        m |> BookListModel, Cmd.map (BookMsg) cmd
    | IndividualListModel m, IndividualMsg ms -> 
        let m, cmd = EntityDefs.individual.Update ms m
        m |> IndividualListModel, Cmd.map (IndividualMsg) cmd
    | OrganizationListModel m, OrganizationMsg ms -> 
        let m, cmd = EntityDefs.organization.Update ms m
        m |> OrganizationListModel, Cmd.map (OrganizationMsg) cmd
    | VesselListModel m, VesselMsg ms -> 
        let m, cmd = EntityDefs.vessel.Update ms m
        m |> VesselListModel, Cmd.map (VesselMsg) cmd
    | BillOfLadingListModel m, BillOfLadingMsg ms -> 
        let m, cmd = EntityDefs.billOfLading.Update ms m
        m |> BillOfLadingListModel, Cmd.map (BillOfLadingMsg) cmd
    | (m, msg) -> 
        Browser.console.error(sprintf "Unexpected Model '%A' and Msg '%A' combination" m msg)
        model, Cmd.none

[<PassGenerics>]
let viewPageStatic<'entity when 'entity: equality> (entityDef: EntityDef<'entity, AppMsg>) (model: EntityModel<'entity>) (dispatch: AppMsg -> unit) =
    [ div [ ] [ lazyView2 entityDef.View model dispatch ]]

let view (sp: Page) (model: Model) (dispatch: AppMsg -> unit) = 
    match sp, model with
        | Book ed, BookListModel m -> viewPageStatic<Book> ed m dispatch
        | Individual ed, IndividualListModel m -> viewPageStatic<Individual> ed m dispatch
        | Organization ed, OrganizationListModel m -> viewPageStatic<Organization> ed m dispatch
        | Vessel ed, VesselListModel m -> viewPageStatic<Vessel> ed m dispatch
        | BillOfLading ed, BillOfLadingListModel m -> viewPageStatic<BillOfLading> ed m dispatch
        | p, m -> 
            Browser.console.error(sprintf "Impossible Page:[%A] - Model:[%A] combination" p m)
            [ ]
    |> div [] 
        

let staticEntityDefs = [
    EntityDefs.individual |> Individual
    EntityDefs.organization |> Organization
    EntityDefs.vessel |> Vessel
    EntityDefs.book |> Book
    EntityDefs.billOfLading |> BillOfLading
]


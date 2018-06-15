module Client.EntityDefs

open Fable.Core
open Fable.Import
open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open ServerCode.Result
open ServerCode.Domain
open ServerCode.Commodities
open ServerCode.EntityList
open System
open Fable.Core.JsInterop
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open FSharp.Reflection

open Client.DomainView

open Style
open Messages 
open Entity

let individual: EntityDef<Individual, AppMsg> = 
    let fields = generateFields<Individual>()
    {   Fields = fields 
        Empty = fun () -> 
                    {   Uid = 0u
                        FirstName = ""
                        LastName = ""
                        MiddleName = "" 
                        PhoneNumber = ""
                        Email = ""
                    }
        Verifier = Validation.verifyEntity (fields)
        Sorter = fun b -> b.LastName
        Title = "Individual"
        MessageCase = IndividualMsg >> StaticsMsg
        ServerUrl = "individual"}

let organization: EntityDef<Organization, AppMsg> =
    let fields = generateFields<Organization>()
    {   Fields = fields 
        Empty = fun () -> { Uid = 0u
                            LegalEntity = { Name = ""
                                            Address = ""
                                            //Contacts = [] 
                                            } 
                            Representative = {  Uid = 0u
                                                FirstName = ""
                                                LastName = ""
                                                MiddleName = ""
                                                PhoneNumber = ""
                                                Email = "" } } |> Ltd
        Verifier = Validation.verifyEntity (fields)
        Sorter = fun b -> b.Name
        Title = "Organization"
        MessageCase = OrganizationMsg >> StaticsMsg
        ServerUrl = "organization" }

let book: EntityDef<Book, AppMsg> = 
    {  //Fields = generateFields<Book>() // Book.Fields
        Fields = Book.Fields
        Empty = fun () -> Book.empty
        Verifier = Book.VerifyBook
        Sorter = fun b -> b.Title 
        Title = "Book"
        MessageCase = BookMsg >> StaticsMsg
        ServerUrl = "book" }

let vesselOperator: VesselOperator = {  Uid = 0u
                                        LegalEntity = { Name = "Big Vessels"
                                                        Address = "190, Oak Street, London, EC4 3AT, UK"
                                                        //Contacts = [] 
                                                        } 
                                        Representative = {  Uid = 0u
                                                            FirstName = "John"
                                                            LastName = "Smith"
                                                            MiddleName = "M"
                                                            PhoneNumber = "+447831010123"
                                                            Email = "john.m.smith@grey.com" } } |> Ltd

let vessel: EntityDef<Vessel, AppMsg> = 
    let fields = generateFields<Vessel>()
    {   Fields = fields 
        Empty = fun () -> 
                    {   Name = ""
                        IMO = Some 0u
                        MMSI = 0u
                        CallSign = ""
                        Flag = ""
                        Built = 0u |> uint16 |> Some
                        // HomePort = ""
                        Capacity = Barrel(100000.<barrels>)
                        // Operator = vesselOperator 
                        VesselType = Barge
                        Captain = { Uid = 100u; FirstName = "Harry"; LastName = "Potter"; MiddleName = ""; PhoneNumber = "+447743567690"; Email = "Harry.Potter@hogwards.com"}
                    }
        Verifier = Validation.verifyEntity (fields)
        Sorter = fun b -> b.Name
        Title = "Vessel"
        MessageCase = VesselMsg >> StaticsMsg
        ServerUrl = "vessel" }

let billOfLading = 
    let fields = generateFields<BillOfLading>()
    {   Fields = fields 
        Empty = fun () -> 
                    {   CargoReferenceNumber = "00000"
                        Seller = organization.Empty()
                        Buyer = organization.Empty()
                        Carrier = vessel.Empty()
                        BoLNumber = "223322223"
                        Quantity = Barrel(1000000.<barrels>)
                        SupportingDocuments = []
                    }
        Verifier = Validation.verifyEntity (fields)
        Sorter = fun b -> b.BoLNumber
        Title = "BillOfLading"
        MessageCase = BillOfLadingMsg >> StaticsMsg
        ServerUrl = "billoflading" }
namespace ServerCode

module Examples =

    open System
    open Commodities 
    open Contracts

    let qualitySpec = { Density = 0.9<kg_m3>; SulphurLevel = 0.0005<percent> }

    let bp = Corporate {Name = "British Petrolium"
                        Headquaters = { LegalEntity = { Name = "BP P.L.C."; Address = "1 St James's Square, St. James's, London SW1Y 4PD" }
                                        Representative = { FirstName = "Bob"; LastName = "Dudley"; MiddleName = ""; PhoneNumber = "+442074964000"; Email = "Bob.Dudley@bp.com"} }
                    }
    let shell = Corporate { Name = "Shell"
                            Headquaters = { LegalEntity = { Name = "Royal Dutch Shell plc"; Address = "1 Canada Square, Canary Wharf, London E14 5AB" }
                                            Representative = { FirstName = "Ben"; LastName = "van Beurden"; MiddleName = ""; PhoneNumber = "+442075465000"; Email = "Ben.van.Beurden@shell.com"} }
                        }

    let terminalLoad = Ltd {LegalEntity = { Name = "Port Rotterdam West"; Address = "Wilhelminakade 909, 3072 AP Rotterdam, Netherlands" }
                            Representative = { FirstName = "Allard"; LastName = "Castelein"; MiddleName = ""; PhoneNumber = "+31102521010"; Email = "Allard.Castelein@portofrotterdam.com"} }
                                
    let terminalDischarge = Ltd {   LegalEntity = { Name = "Port Amsterdam South"; Address = "De Ruijterkade 7 1013 AA Amsterdam, Netherlands" }
                                    Representative = { FirstName = "Dertje"; LastName = "Meijer"; MiddleName = ""; PhoneNumber = "+31206209821"; Email = "Dertje.Meijer@portofamsterdam.com"} }

    let inspector = Ltd {   LegalEntity = { Name = "Diesel Barge Inspection Ltd"; Address = "Eiruijter 12 2057 AA Amsterdam, Netherlands" }
                            Representative = { FirstName = "John"; LastName = "Smith"; MiddleName = "J"; PhoneNumber = "+31222332232"; Email = "John.J.Smith@bargeinspect.nl"} }

    let vesselOperator = Ltd {  LegalEntity = { Name = "Barges and Co."; Address = "1 Canada Square, Canary Wharf, London, UK" }
                                Representative = { FirstName = "Simon"; LastName = "Price"; MiddleName = "W"; PhoneNumber = "+44202355537"; Email = "Simon.W.Price@barges.co.uk"} }


    let terminalLoad2 = Ltd {LegalEntity = { Name = "Port Rotterdam West2"; Address = "Wilhelminakade 909, 3072 AP Rotterdam, Netherlands" }
                             Representative = { FirstName = "Stephan"; LastName = "Castelein"; MiddleName = ""; PhoneNumber = "+31102521010"; Email = "Allard.Castelein@portofrotterdam.com"} }
                                
    let terminalDischarge2 = Ltd {  LegalEntity = { Name = "Port Amsterdam South2"; Address = "De Ruijterkade 7 1013 AA Amsterdam, Netherlands" }
                                    Representative = { FirstName = "Janned"; LastName = "Meijer"; MiddleName = ""; PhoneNumber = "+31206209821"; Email = "Dertje.Meijer@portofamsterdam.com"} }

    let inspector2 = Ltd {  LegalEntity = { Name = "Diesel Barge Inspection Ltd2"; Address = "Eiruijter 12 2057 AA Amsterdam, Netherlands" }
                            Representative = { FirstName = "Eik"; LastName = "Smith"; MiddleName = "J"; PhoneNumber = "+31222332232"; Email = "John.J.Smith@bargeinspect.nl"} }

    let vesselOperator2 = Ltd { LegalEntity = { Name = "Barges and Co.2"; Address = "1 Canada Square, Canary Wharf, London, UK" }
                                Representative = { FirstName = "Ruppert"; LastName = "Price"; MiddleName = "W"; PhoneNumber = "+44202355537"; Email = "Simon.W.Price@barges.co.uk"} }


    let captain = { FirstName = "Harry"; LastName = "Potter"; MiddleName = ""; PhoneNumber = "+447743567690"; Email = "Harry.Potter@hogwards.com"}

    let portLoad = {
        Name = "Rotterdam"
        LOCODE = [ "TODO" ]
        Terminals = [ terminalLoad ]
        Tide = 20.<m>
        ChannelDepth = 14.<m>
        AnchorageDepth = 14.<m>
        CargoPierDepth = 7.<m>
        OilTerminalDepth = 8.<m>
    }

    let portDischarge = {
        Name = "Amsterdam"
        LOCODE = [ "TODO" ]
        Terminals = [ terminalDischarge ]
        Tide = 25.<m>
        ChannelDepth = 18.<m>
        AnchorageDepth = 17.<m>
        CargoPierDepth = 9.<m>
        OilTerminalDepth = 12.<m>
    }

    let vessel = {
        Name = "CARPEDIEM"
        Capacity = Mt 100000.<mts> 
        Operator = vesselOperator
        IMO = None
        MMSI = 244660476u
        CallSign = "CARPED"
        Flag = "Netherlands"
        VesselType = VesselType.Barge
        Built = Some 1978us
        //AisVesselType: AisVesselType
    }

    let vessel2 = {
        Name = "UPORDUM"
        Capacity = Mt 200000.<mts> 
        Operator = vesselOperator
        IMO = None
        MMSI = 544660476u
        CallSign = "CARPED"
        Flag = "Netherlands"
        VesselType = VesselType.Barge
        Built = Some 1978us
        //AisVesselType: AisVesselType
    }

    let vessel3 = {
        Name = "QUADIUM"
        Capacity = Mt 300000.<mts> 
        Operator = vesselOperator
        IMO = None
        MMSI = 944660476u
        CallSign = "CARPED"
        Flag = "Netherlands"
        VesselType = VesselType.Barge
        Built = Some 1978us
        //AisVesselType: AisVesselType
    }

    let operatorFleet = { Operator = vesselOperator; Fleet = [ vessel; vessel2; vessel3 ] }

    let seller = bp
    let buyer = shell

    let contractTerms = {   ContractType = FOB
                            Seller = seller
                            Buyer = buyer
                            Inspector = inspector
                            Price = 1000000.0<usd>
                            QualitySpecs = qualitySpec
                            Quantity = Mt 1000.0<mts>
                            TerminalLoad = terminalLoad
                            TerminalDischarge = terminalDischarge
                            DateLoad = DateTime.Today.AddDays 3.0
                            DateDischarge = DateTime.Today.AddDays 10.0
                            Terms = [| "Standard Agreement" |]
                        }

    let vesselOffer = { Vessel = vessel
                        Price = 10000.<usd>
                        AvailabilityDate = DateTime.Today
                        }

    let inspectionResults = {
        ActualDeliveryDate = DateTime.Today
        QualitySpecs = qualitySpec
        Quantity = Mt 999.0<mts>
    }

    let bolInfo = {
        BoLInfo.CargoReferenceNumber = "#Load_Cargo_Ref_Number#"
        BoLNumber = "#Load_BOL_Number#"
        Volume = Mt 90000.<mts>
    }

    let bolInfoDischarge = {
        BoLInfo.CargoReferenceNumber = "#Discharge_Cargo_Ref_Number#"
        BoLNumber = "#Discharge_BOL_Number#"
        Volume = Mt 85000.<mts>
    }

    let releaseDischargeDocumentsDefault() = releaseDischargeDocuments bolInfoDischarge


    /////

    let issueInvoice invoiceDate = 
        progressFinancialLeg <| function
                                | ReadyForInvoicing fcs -> fcs.``Send Invoice`` invoiceDate (sellerSign invoiceDate) |> Some
                                | _ -> None

    let issueInvoiceDefault() = issueInvoice (DateTime.Today.AddDays 5.)

    /////

    let payInvoice (settlementInfo: SettlementInfo) = 
        progressFinancialLeg <| function
                                | InvoiceCreated fcs -> fcs.``Pay Invoice`` settlementInfo (buyerSign settlementInfo) |> Some
                                | _ -> None

    let payInvoiceDefault() = payInvoice { PayDate = (DateTime.Today.AddDays 5.); SettlementDate = (DateTime.Today.AddDays 8.) }


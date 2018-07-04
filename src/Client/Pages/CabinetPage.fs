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
open Client.LoginCommon
open Fable.Import.JS
open Fable
open Elmish.Browser
open Elmish.Toastr

// type Model = 
//     | VerificationModel     of string
//     | PurchaseTokenModel    of ViewModels.TokenSale option
//     | MyInvestmentsModel    of string
//     | ReferralProgramModel  of string
//     | ContactsModel         of string
//     | DashboardModel        of string




let init authToken = 
    {   Auth                    = { Token = authToken }
        CryptoCurrencies        = []
        CurrenciesCurentPrices  = { Prices = [] }
        ActiveSymbol            = ETH
        TokenSale               = None
        FullCustomer            = None 
        PurchaseTokenModel      = { CCTokens = 10m
                                    BuyTokens = 0m
                                    TotalPrice = 0m
                                    // CCPrice = 0m 
                                    }
    }

   

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

module Option = 
    let map2 f a b = match a, b with    | Some a, Some b -> f a b |> Some 
                                        | _ -> None

let update (msg: Msg) model : Model * Cmd<Msg> = //model ,Cmd.none
    
    let ccTotalPrice activeSymbol cCTokens (tick: ViewModels.CurrencyPriceTick) =
        tick.Prices 
        |> List.tryFind(fun p -> p.Symbol = activeSymbol)
        |> Option.map(fun p -> p.PriceUsd * cCTokens)
        |> Option.defaultValue 0m

    let tokenTotalPrice (model: ViewModels.TokenSale option) activeSymbol tokens (tick: ViewModels.CurrencyPriceTick) =
        
        let tokensPrice (m: ViewModels.TokenSale option) = 
            match m with 
            | Some t -> t.TokenSaleState.PriceUsd
            | None -> 0m

        tick.Prices 
        |> List.tryFind(fun p -> p.Symbol = activeSymbol)
        |> Option.map(fun p -> (tokensPrice model) * tokens)
        |> Option.defaultValue 0m

    let tokenPrice activeSymbol ccTokens (tick: ViewModels.CurrencyPriceTick) = 
        tick.Prices 
        |> List.tryFind(fun p -> p.Symbol = activeSymbol)
        |> Option.map2(fun (m: ViewModels.TokenSale) p -> (p.PriceUsd * ccTokens) / m.TokenSaleState.PriceUsd ) model.TokenSale
        |> Option.defaultValue 0m
    
    let ccPrice activeSymbol tokens (tick: ViewModels.CurrencyPriceTick) = 
        tick.Prices 
        |> List.tryFind(fun p -> p.Symbol = activeSymbol)
        |> Option.map2(fun (m: ViewModels.TokenSale) p -> (m.TokenSaleState.PriceUsd * tokens) / p.PriceUsd ) model.TokenSale
        |> Option.defaultValue 0m

    match msg with
    | VerificationMsg    -> model, Cmd.none
    | PurchaseTokenMsg msg_  -> 
        match msg_ with 
        | ActiveSymbolChanged symbol    -> { model with ActiveSymbol = symbol }, model.PurchaseTokenModel.BuyTokens |> TAmountChanges |> PurchaseTokenMsg |> Cmd.ofMsg
        | CCAmountChanges ccTokens      -> 
            { model with PurchaseTokenModel = 
                                                        { model.PurchaseTokenModel with CCTokens = ccTokens 
                                                                                        BuyTokens = tokenPrice model.ActiveSymbol ccTokens model.CurrenciesCurentPrices
                                                                                        TotalPrice = ccTotalPrice model.ActiveSymbol ccTokens model.CurrenciesCurentPrices } }, Cmd.none 
        | TAmountChanges tokens         -> { model with PurchaseTokenModel = 
                                                        { model.PurchaseTokenModel with BuyTokens = tokens 
                                                                                        CCTokens = ccPrice model.ActiveSymbol tokens model.CurrenciesCurentPrices
                                                                                        TotalPrice = tokenTotalPrice model.TokenSale model.ActiveSymbol tokens model.CurrenciesCurentPrices } }, Cmd.none 
        
    | MyInvestmentsMsg   -> model, Cmd.none
    | ReferralProgramMsg -> model, Cmd.none
    | ContactsMsg        -> model, Cmd.none
    | DashboardMsg       -> model, Cmd.none
    | ServerMsg msg_     ->
        match msg_ with
        | GetCryptoCurrenciesCompleted cc   -> { model with CryptoCurrencies = cc } , Cmd.none
        | GetTokenSaleCompleted tc          -> { model with TokenSale = Some (tc) } , Cmd.none
        | GetFullCustomerCompleted fc       -> { model with FullCustomer = Some (fc) } , Cmd.none
        | PriceTick tick                    -> { model with CurrenciesCurentPrices = tick
                                                            PurchaseTokenModel = 
                                                            { model.PurchaseTokenModel with TotalPrice = ccTotalPrice model.ActiveSymbol model.PurchaseTokenModel.CCTokens tick } }, Cmd.none

    // | BookListModel m, BookMsg ms -> 
    //     let m, cmd = EntityDefs.book.Update ms m
    //     m |> BookListModel, Cmd.map (BookMsg) cmd


    // | (m, msg) -> 
    //     Browser.console.error(sprintf "Unexpected Model '%A' and Msg '%A' combination" m msg)
    //     model, Cmd.none

open ReactBootstrap
open Fable.Import.React
open Client.Helpers
open Page


let view (page: CabinetPagePage) (model: Model) (dispatch: Msg -> unit) = 
    match page with
        | Verification      -> [ VerificationPage.view ]
        | PurchaseToken     -> [ PurchaseTokenPage.view model dispatch]
        | Investments       -> [ InvestmentPage.view ]
        | ReferralProgram   -> [ ReferralProgramPage.view ]
        | Contacts          -> [ ContactsPage.view ]
        // | Dashboard          -> [ DashboardView.view ]
        // | Book ed, BookListModel m -> viewPageStatic<Book> ed m dispatch
        // | Individual ed, IndividualListModel m -> viewPageStatic<Individual> ed m dispatch
        // | Organization ed, OrganizationListModel m -> viewPageStatic<Organization> ed m dispatch
        // | Vessel ed, VesselListModel m -> viewPageStatic<Vessel> ed m dispatch
        // | BillOfLading ed, BillOfLadingListModel m -> viewPageStatic<BillOfLading> ed m dispatch
        // | p -> 
        //     Browser.console.error(sprintf "Impossible Page:[%A] - Model:[%A] combination" p)
        //     [ ]
    |> div [] 
        




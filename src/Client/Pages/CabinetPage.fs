module Client.CabinetPage

open System

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
// open Fable.PowerPack.Fetch.Fetch_types

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
open Helpers

// type Model = 
//     | VerificationModel     of string
//     | PurchaseTokenModel    of ViewModels.TokenSale option
//     | MyInvestmentsModel    of string
//     | ReferralProgramModel  of string
//     | ContactsModel         of string
//     | DashboardModel        of string


open Web3
open Web3Types
open Fable.Import.BigNumber

[<Emit("window.web3")>]
let web3: Web3 = jsNative
// console.log (sprintf "web3: '%A'" web3)
// console.log (sprintf "web3cp: '%A'" web3.currentProvider)
let IsWeb3 = isNull web3

console.log (sprintf "IsW3: '%A'" IsWeb3)
// let w3 = web3Factory.Create("http://127.0.0.1:8545" |> U2.Case2 )
let w3 = web3Factory.Create(web3.currentProvider |> U2.Case1 )



let init authToken = 
    {   Auth                    = { Token = authToken }
        CryptoCurrencies        = []
        CurrenciesCurentPrices  = { Prices = [] }
        ActiveSymbol            = ETH
        TokenSale               = None
        FullCustomer            = None 
        PurchaseTokenModel      = { CCTokens = 0m
                                    BuyTokens = 0m
                                    TotalPrice = 0m
                                    CCAddress = "" 
                                    IsLoading = false
                                    }
        VerificationModel       = {
                                    CurrentTab = 1
                                    }
        IsWeb3                  = IsWeb3
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



let update (msg: Msg) model : Model * Cmd<Msg> = //model ,Cmd.none
    
    let ccTotalPrice activeSymbol cCTokens (tick: ViewModels.CurrencyPriceTick) = 
        PurchaseTokenPage.calcPrice activeSymbol tick (Option.map(fun p -> p.PriceUsd * cCTokens))

    let tokenTotalPrice (model: ViewModels.TokenSale option) activeSymbol tokens (tick: ViewModels.CurrencyPriceTick) =
        
        let tokensPrice (m: ViewModels.TokenSale option) = 
            match m with 
            | Some t -> t.TokenSaleState.PriceUsd
            | None -> 0m

        PurchaseTokenPage.calcPrice activeSymbol tick (Option.map(fun _ -> (tokensPrice model) * tokens))

    let tokenPrice activeSymbol ccTokens (tick: ViewModels.CurrencyPriceTick) = 
        PurchaseTokenPage.calcPrice activeSymbol tick (Option.map2(fun (m: ViewModels.TokenSale) p -> (p.PriceUsd * ccTokens) / m.TokenSaleState.PriceUsd ) model.TokenSale)
    
    let ccPrice activeSymbol tokens (tick: ViewModels.CurrencyPriceTick) = 
        PurchaseTokenPage.calcPrice activeSymbol tick (Option.map2(fun (m: ViewModels.TokenSale) p -> (m.TokenSaleState.PriceUsd * tokens) / p.PriceUsd ) model.TokenSale)

    let changeCurrentAddress symbol (fc:ViewModels.FullCustomer) = 
        (fc.Wallet.ForSymbol symbol).Address.Value
    
    let buyTokens amount (fc:ViewModels.FullCustomer option) = 
        let account (fc: ViewModels.FullCustomer option) = 
                match fc with 
                | Some ac -> ac.Wallet.Accounts.Etc.Address.Value.ToString()
                | None -> ""
        let toAccount = account fc

        let sendToChain () =
            promise {
                let! coinbase = w3.eth.getCoinbase()
                console.log "coinbase"
                console.log (coinbase)
                let amount = w3.utils.toWei( amount |> U3.Case1, Web3Types.Unit.Ether )
                
                let tr = jsOptions<Tx>(fun  tx -> tx.value <- amount |> Some 
                                                  tx.from <- Some coinbase
                                                  tx.``to`` <- Some toAccount )
                 
                let! tx = w3.eth.sendTransaction tr
                let! balance = w3.eth.getBalance(toAccount)

                console.log "getBalanse"
                console.log (balance)

                let! accs = w3.eth.getAccounts()
                console.log accs
                return tx
            }
        if toAccount.Length > 0 
        then    
            Cmd.ofPromise sendToChain () 
                                (fun (tx) -> 
                                     SignResult (Ok (tx.transactionHash)) |> PurchaseTokenMsg) 
                                (fun e ->      //console.error "Failed sent"
                                     (SignResult (Result.Error e.Message)) |> PurchaseTokenMsg ) 
        else
            toastrError "Account not found" 

    let isConnecting model (status:bool )= { model.PurchaseTokenModel with IsLoading =  status  } 
    
    match msg with
    | VerificationMsg  msg_  -> 
        match msg_ with
        | TabChanged activeKey -> 
            let verifiacationModel = { model.VerificationModel with CurrentTab = activeKey }
            { model with VerificationModel = verifiacationModel } , Cmd.none
    | PurchaseTokenMsg msg_  -> 
        match msg_ with 
        | ActiveSymbolChanged symbol    -> 
            let purchaseTokenModel = { model.PurchaseTokenModel with CCAddress = (model.FullCustomer |> Option.map (changeCurrentAddress symbol) |> Option.defaultValue "")  }
            { model with    ActiveSymbol = symbol 
                            PurchaseTokenModel = purchaseTokenModel }, model.PurchaseTokenModel.BuyTokens |> TAmountChanges |> PurchaseTokenMsg |> Cmd.ofMsg
        | CCAmountChanges ccTokens      -> 
            { model with PurchaseTokenModel = 
                                                        { model.PurchaseTokenModel with CCTokens = ccTokens 
                                                                                        BuyTokens = tokenPrice model.ActiveSymbol ccTokens model.CurrenciesCurentPrices
                                                                                        TotalPrice = ccTotalPrice model.ActiveSymbol ccTokens model.CurrenciesCurentPrices } }, Cmd.none 
        | TAmountChanges tokens         -> { model with PurchaseTokenModel = 
                                                        { model.PurchaseTokenModel with BuyTokens = tokens 
                                                                                        CCTokens = ccPrice model.ActiveSymbol tokens model.CurrenciesCurentPrices
                                                                                        TotalPrice = tokenTotalPrice model.TokenSale model.ActiveSymbol tokens model.CurrenciesCurentPrices } }, Cmd.none 
        | AddressCopied text -> model, toastrSuccess (sprintf "Address Copied") 
        | BuyTokens -> 
            let cmd = buyTokens (model.PurchaseTokenModel.CCTokens.ToString()) model.FullCustomer 
            { model with PurchaseTokenModel = (isConnecting model true) }, cmd 
        | SignResult signResult -> 
            { model with PurchaseTokenModel = (isConnecting model false) },  
            match signResult with 
            | Ok transaction -> toastrSuccess ("Signed succesfully! Transaction index: " + transaction)
            | Error message -> toastrError ("Exception during Metamask call: " + message)

    | MyInvestmentsMsg   -> model, Cmd.none
    | ReferralProgramMsg -> model, Cmd.none
    | ContactsMsg        -> model, Cmd.none
    | DashboardMsg       -> model, Cmd.none
    | ServerMsg msg_     ->
        match msg_ with
        | GetCryptoCurrenciesCompleted cc   -> { model with CryptoCurrencies = cc } , Cmd.none
        | GetTokenSaleCompleted tc          -> { model with TokenSale = Some (tc) } , Cmd.none
        | GetFullCustomerCompleted fc       -> { model with FullCustomer = Some (fc)  
                                                            PurchaseTokenModel = { model.PurchaseTokenModel with CCAddress = changeCurrentAddress model.ActiveSymbol fc } }, Cmd.none
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
        | Verification      -> [ VerificationPage.view model dispatch ]
        | PurchaseToken     -> [ PurchaseTokenPage.view model dispatch ]
        | Investments       -> [ InvestmentPage.view model dispatch ]
        | ReferralProgram   -> [ ReferralProgramPage.view model dispatch ]
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
        




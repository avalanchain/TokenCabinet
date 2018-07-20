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
open Shared
open Client.CabinetModel
open Client.LoginCommon
open Fable.Import.JS
open Fable
open Elmish.Browser
open Elmish.Toastr
open Helpers

open Web3
open Web3Types
open Fable.Import.BigNumber

[<Emit("window.web3")>]
let web3: Web3 = jsNative
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

let update (msg: Msg) model : Model * Cmd<Msg> = 
    
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

let view (page: Page.CabinetPage) (model: Model) (dispatch: Msg -> unit) = 
    match page with
        | Page.CabinetPage.Verification      -> [ VerificationPage.view model dispatch ]
        | Page.CabinetPage.PurchaseToken     -> [ PurchaseTokenPage.view model dispatch ]
        | Page.CabinetPage.Investments       -> [ InvestmentPage.view model dispatch ]
        | Page.CabinetPage.ReferralProgram   -> [ ReferralProgramPage.view model dispatch ]
        | Page.CabinetPage.Contacts          -> [ ContactsPage.view ]
    |> div [] 
        




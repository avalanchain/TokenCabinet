module Client.CabinetPage

open System

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.JS
open Fable.Import.BigNumber
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
// open Fable.PowerPack.Fetch.Fetch_types

open Elmish
open Elmish.React
open Elmish.React.Common
open Elmish.Browser
open Elmish.Browser.Navigation
open Elmish.Toastr

open Shared
open Helpers
open ClientMsgs
open ClientModels
open Client.CabinetModel
open Client.LoginCommon
open Client.Cabinet

open Web3
open Web3Types

[<Emit("window.web3")>]
let web3: Web3 = jsNative

let IsWeb3 = web3 |> isNull |> not && (!!web3.currentProvider |> isNull |> not)

let w3Opt = 
    if IsWeb3 then  
        let w3 = web3Factory.Create(web3.currentProvider |> U2.Case1 )

        promise {
            let! accs = w3.eth.getAccounts()
            console.log "accs"
            console.log accs
            let! bal = w3.eth.getBalance(accs.[0])
            console.log "bal"
            console.log (bal / 1000000000000000000.)
            let! coinbase = w3.eth.getCoinbase()
            console.log "coinbase"
            console.log (coinbase)
            let amount = w3.utils.toWei("1" |> U3.Case1, Web3Types.Unit.Ether)
            
            let! transactions = w3.eth.getTransactionCount(coinbase)
            console.log "transactions"
            console.log transactions

            let defaultAccount = web3.eth.defaultAccount
            console.log "defaultAccount"
            console.log defaultAccount
            // let provider = web3.currentProvider :> obj :?> IProvider
            // let! _ = provider.send(jsOptions<JsonRPCRequest>(fun r ->  r.method <- "personal_sign" |> Some 
            //                                                            r.``to`` <- Some "0xC25FdBeaD74a9A1d09F3209d2fcDE652d4D359fE" )) 

            let tr = jsOptions<Tx>(fun  tx -> tx.value <- amount |> Some 
                                              tx.from <- Some coinbase
                                              tx.``to`` <- Some "0xC25FdBeaD74a9A1d09F3209d2fcDE652d4D359fE" )
         
            let! accs = w3.eth.getAccounts()
            console.log accs
        }
        |> PowerPack.Promise.start
        Some w3
    else None

let init authToken = 
    let model = {   Auth                    = { Token = authToken }
                    CryptoCurrencies        = []
                    CurrenciesCurentPrices  = { Prices = [] }
                    ActiveSymbol            = ETH
                    TokenSale               = None
                    FullCustomer            = None 
                    Transactions            = None
                    PurchaseTokenModel      = { CCTokens = 0m
                                                BuyTokens = 0m
                                                TotalPrice = 0m
                                                CCAddress = "" 
                                                IsLoading = false
                                                }
                    VerificationModel       = { CurrentTab = 1
                                                }
                    InvestmentModel         = { Coinbase = ""
                                                Transactions = []
                                                IsLoading = false
                                                }
                    IsWeb3                  = IsWeb3
                    // Coinbase                = w3.eth.getCoinbase()
    }
    let cmdGetCryptoCurrencies      = cmdServerCabinetCall (ServerProxy.cabinetApi.getCryptoCurrencies) () GetCryptoCurrenciesCompleted "getCryptoCurrencies()"
    let cmdGetTokenSale             = cmdServerCabinetCall (ServerProxy.cabinetApi.getTokenSale) () GetTokenSaleCompleted "getTokenSale()"
    let cmdGetFullCustomerCompleted = cmdServerCabinetCall (ServerProxy.cabinetApi.getFullCustomer) (Auth.secureVoidRequest authToken) GetFullCustomerCompleted "getFullCustomer()"
    let cmdGetTransactionsCompleted = cmdServerCabinetCall (ServerProxy.cabinetApi.getTransactions) (Auth.secureVoidRequest authToken) GetTransactionsCompleted "getTransactions()"
    let cmd = Cmd.batch [   cmdGetCryptoCurrencies
                            cmdGetTokenSale
                            cmdGetFullCustomerCompleted
                            cmdGetTransactionsCompleted ]
    model, cmd

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
    
    let account (fc: ViewModels.FullCustomer option) = 
                match fc with 
                | Some ac -> ac.Wallet.Accounts.Etc.Address.Value.ToString()
                | None -> ""

    let buyTokens amount (fc:ViewModels.FullCustomer option) = 
        match w3Opt with
        | Some w3 ->
            let toAccount = account fc
            let sendToChain () =
                promise {
                    let! coinbase = w3.eth.getCoinbase()
                    // console.log "coinbase"
                    // console.log (coinbase)
                    let amount = w3.utils.toWei( amount |> U3.Case1, Web3Types.Unit.Ether )
                    
                    let tr = jsOptions<Tx>(fun  tx -> tx.value <- amount |> Some 
                                                      tx.from <- Some coinbase
                                                      tx.``to`` <- Some toAccount )
                     
                    let! tx = w3.eth.sendTransaction tr
                    // let! balance = w3.eth.getBalance(toAccount)

                    // console.log "getBalance"
                    // console.log (balance)

                    // let! accs = w3.eth.getAccounts()
                    // console.log accs
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
        | None -> toastrError "Web3 provider is not available"

    let getTransactions account = 
        match w3Opt with
        | Some w3 -> 
            let getTransactionCount () =
                promise {
                    
                    let! count = w3.eth.getTransactionCount(account)
                    console.log "geting transactions.."

                    let! endBlockNumber = w3.eth.getBlockNumber()
                    console.log(endBlockNumber)
                    let startBlockNumber  = if endBlockNumber < 1000. then 0. else 0.
                    console.log(startBlockNumber)

                    let transactions = ResizeArray<Transaction>()

                    for i in startBlockNumber .. endBlockNumber do

                        let! block = w3.eth.getBlock(i |> U4.Case4, true)
                        
                        if not (isNull block) && not (isNull block.transactions)
                        then 
                            block.transactions.forEach(Func<_,_,_,_>(fun t i trs -> if ((t.``to``.ToLowerInvariant()) = account.ToLowerInvariant())
                                                                                    then transactions.Add(t)))
                            //  transactions.Add(t)
                    console.log (transactions |> Seq.toArray)            
                        // else

                    // if (count = 0.)
                    // then transactions
                    // else transactions = []
                    let ts: Transaction list = transactions |> Seq.toList
                    return ts 
                }
            Cmd.ofPromise getTransactionCount () 
                                    ( Ok >> TransactionsResult  >> InvestmentsMsg ) 
                                    (fun e -> e.Message |>  Result.Error |> TransactionsResult |> InvestmentsMsg )
        | None -> [] |> Ok |> TransactionsResult  |> InvestmentsMsg |> Cmd.ofMsg                                                
                                      

    let getCoinbase = 

        Cmd.ofPromise (fun () -> match w3Opt with Some w3 -> w3.eth.getCoinbase() | None -> promise { return "0x0" }) ()
                                ( Ok >> CoinbaseResult  >> InvestmentsMsg ) 
                                (fun e -> (CoinbaseResult (Result.Error e.Message)) |> InvestmentsMsg )    

   
    let isConnecting model (status:bool )= { model.PurchaseTokenModel with IsLoading =  status  } 
    let metemaskError text = toastrError ("Exception during Metamask call: " + text)

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
            | Ok transaction -> toastrSuccess ("Signed succesfully! Transaction hash: " + transaction)
            | Error message -> toastrError ("Exception during Metamask call: " + message)

    | InvestmentsMsg msg_  ->
        match msg_ with  
        | GetCoinbase  -> model, getCoinbase
        | GetTransactions c -> model, Cmd.none //getTransactions c
        | GetTransactionCount  -> model, Cmd.none
        | CoinbaseResult res -> 
            match res with 
            | Ok c -> { model with InvestmentModel = { model.InvestmentModel with Coinbase =  c
                                                                                  IsLoading = false  } }, Cmd.none //GetTransactions c |> InvestmentsMsg |> Cmd.ofMsg  //getTransactions c
            | Error message -> { model with InvestmentModel = { model.InvestmentModel with IsLoading = false  } }, 
                                                                metemaskError message
        | TransactionsResult res -> 
            // { model with PurchaseTokenModel = (isConnecting model false) },  
            match res with 
            | Ok ts -> 
                let cmd = toastrSuccess ("Transactions got ")
                { model with InvestmentModel = { model.InvestmentModel with Transactions =  ts  } }, cmd
            | Error message -> model, metemaskError message


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
        | GetTransactionsCompleted ts       -> { model with Transactions = Some (ts) }, Cmd.none
        
let view (page: Cabinet.MenuPage) (model: Model) (dispatch: Msg -> unit) = 
    match page with
        | Cabinet.MenuPage.Verification      -> [ VerificationPage.view model dispatch ]
        | Cabinet.MenuPage.PurchaseToken     -> [ PurchaseTokenPage.view model dispatch ]
        | Cabinet.MenuPage.Investments       -> [ InvestmentPage.view model dispatch ]
        | Cabinet.MenuPage.ReferralProgram   -> [ ReferralProgramPage.view model dispatch ]
        | Cabinet.MenuPage.Contacts          -> [ ContactsPage.view ]
    |> div [] 
        



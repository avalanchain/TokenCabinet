module Cabinet

open System
open System.Threading.Tasks
open FSharp.Control.Tasks.V2.ContextInsensitive
open FSharp.Data

open Shared
open Shared.ViewModels
open Shared.Auth
open Shared.Utils
open Shared.Result
open Shared.WsBridge

open TypeShape.Tools

open Elmish
open Elmish.Bridge

open AuthJwt
open Config
open Customer.Wallet
open Customer.Transactions
open Customer
open LoginFlow

let getCryptoCurrencies config () = task { 
    printfn "getCryptoCurrencies() called"
    let! res = CryptoCurrencies.Database.getAll(config.connectionString) 
    return match res with
            | Ok o -> [ for cc in o -> {Symbol  = (getUnionCaseFromString<CryptoCurrencySymbol> cc.Id).Value // TODO: Do a proper failwith
                                        Name    = cc.Name
                                        LogoUrl = cc.LogoUrl } ] |> Ok
            | Error exn ->  printfn "Data access exception: '%A'" exn
                            exn |> InternalError |> Error
}

let getAllFromDb<'T,'U> config (getAll: string -> Task<Result<seq<'T>, exn>>) (f: 'T -> 'U) : Task<Result<'U list, ServerError>> = task {
    printfn "get%s() called" typeof<'T>.Name
    let! res = getAll config.connectionString 
    return match res with
            | Ok o -> o |> Seq.map f |> List.ofSeq |> Ok
            | Error exn ->  printfn "Data access exception: '%A'" exn
                            exn |> InternalError |> Error
}

let getSaleToken config = task {
    let! st = getAllFromDb config SaleTokens.Database.getAll (fun t -> {    Symbol     = t.Id
                                                                            Name       = t.Name
                                                                            LogoUrl    = t.LogoUrl
                                                                            TotalSupply = t.TotalSupply })
    return st |> Result.map Seq.head } // SaleToken should be 1 record always

let unwrapResult res = 
    match res with 
    | Ok r -> r
    | Error e -> match (box e) with 
                    | :? Exception as exn -> raise exn
                    | exn -> failwithf "Error: '%A'" exn

let unwrapResultOpt res = 
    match res with 
    | Ok (Some r) -> r
    | Ok None -> failwithf "Incorrect Database setup"
    | Error exn -> raise exn

type BridgeConnectionState =
    | Connected of AuthToken
    | Disconnected
   
type EthNetState =
    | Exist 
    | Not

let bridgeConnections =
    ServerHub<BridgeConnectionState, WsBridge.ServerMsg, WsBridge.BridgeMsg>.New()    


let getTokenSaleStages config = task {  
    let processStage (stage: TokenSaleStages.TokenSaleStage) = task {
        printfn "Stage Id: %A" stage.Id
        let! statusRes = TokenSaleStageStatuses.Database.getByStageId config.connectionString stage.Id
        printfn "StatusRes: %A" statusRes
        let status = (unwrapResultOpt statusRes).Status
        return {TokenSaleStage.Id        = stage.Id
                TokenSaleStage.Name      = stage.Name
                TokenSaleStage.CapEth    = stage.CapEth
                TokenSaleStage.CapUsd    = stage.CapUsd
                TokenSaleStage.StartDate = stage.StartDate
                TokenSaleStage.EndDate   = stage.EndDate
                TokenSaleStage.Status    = match getUnionCaseFromString status with 
                                            | Some s -> s 
                                            | None -> failwithf "Unsupported status %s for TokenSaleStage %d" status stage.Id }
    }

    return! getAllFromDb config TokenSaleStages.Database.getAll processStage
} 

let getTokenSaleStatus config (tokenSaleStages: TokenSaleStage []) = task {
    let processStatus (stage: TokenSaleStatuses.TokenSaleStatus) = 
        {   TokenSaleStatus = TokenSaleStatus.Active
            ActiveStage     = tokenSaleStages |> Array.find (fun s -> s.Id = stage.ActiveStageId)
            PriceUsd        = stage.PriceUsd
            PriceEth        = stage.PriceEth
            BonusPercent    = stage.BonusPercent
            BonusTokens     = stage.BonusTokens
            StartDate       = stage.StartDate
            EndDate         = stage.EndDate }
    let! st = getAllFromDb config TokenSaleStatuses.Database.getAll processStatus
    return st |> Result.map Seq.head 
} // SaleToken should be 1 record always

let getTokenSaleRecord config saleToken tokenSaleState (tokenSaleStages: TokenSaleStage []) = task {
    let processSale (sale: TokenSales.TokenSale) = 
        {   SaleToken   = saleToken
            SoftCapEth  = sale.SoftCapEth
            HardCapEth  = sale.HardCapEth
            SoftCapUsd  = sale.SoftCapUsd
            HardCapUsd  = sale.HardCapUsd
            Expectations = sale.Expectations
            StartDate   = sale.StartDate
            EndDate     = sale.EndDate
            
            TokenSaleState = tokenSaleState

            TokenSaleStages = tokenSaleStages |> List.ofArray }
    let! st = getAllFromDb config TokenSales.Database.getAll processSale
    return st |> Result.map Seq.head 
} // SaleToken should be 1 record always


let getTokenSale config () = task { 
    printfn "getTokenSale() called"

    let! saleTokenRes = getSaleToken config
    let saleToken = saleTokenRes |> unwrapResult

    let! tokenSaleStagesRes = getTokenSaleStages config 
    let tokenSaleStagesTasks = tokenSaleStagesRes |> unwrapResult |> Array.ofList  
    
    let! tokenSaleStages = tokenSaleStagesTasks |> Task.WhenAll  

    let! tokenSaleStateRes = getTokenSaleStatus config tokenSaleStages 
    let tokenSaleState = tokenSaleStateRes |> unwrapResult

    let! tokenSale = getTokenSaleRecord config saleToken tokenSaleState tokenSaleStages

    return tokenSale 
}                                

let getCustomerPreferences config = task {
    let processStatus (prefs: CustomerPreferences.CustomerPreference) = 
        {   CustomerId = prefs.Id |> Guid.Parse
            Language   = prefs.Language }
    let! st = getAllFromDb config CustomerPreferences.Database.getAll processStatus
    return st |> Result.map Seq.head 
} // SaleToken should be 1 record always

let getCustomer config = task {
    let processStatus (customer: Customers.Customer) = 
        {   Id          = Guid.Parse(customer.Id)
            Email       = customer.Email
            FirstName   = customer.FirstName
            LastName    = customer.LastName
            EthAddress  = customer.EthAddress
            Avatar      = customer.Avatar
        }, customer
    let! st = getAllFromDb config Customers.Database.getAll processStatus
    return st |> Result.map Seq.head 
} 

let getWallet config (customerId: Guid) = task {
    let! walletRes = WalletsKV.Database.getById config.connectionString (customerId.ToString("N"))
    let walletOpt = walletRes |> unwrapResult
    return! match walletOpt with
            | Some wallet -> wallet.Wallet |> Json.fromJson |> Task.FromResult
            | None -> task {
                    let wallet = createCustomerWallet customerId
                    let walletKV: WalletsKV.WalletKV = {CustomerId  = customerId.ToString("N")
                                                        Wallet      = Json.toJson wallet } 
                    let! _ = walletKV |> WalletsKV.Database.insert config.connectionString
                    return wallet 
                }
}

let  getFullCustomer config (request: SecureVoidRequest) = task { 
    printfn "getFullCustomer() called"

    return! 
        if request.Token |> isTokenValid |> not then TokenInvalid |> AuthError |> Error |> Task.FromResult
        elif request.Token |> checkAuthTokenValid config |> not then UserDoesNotHaveAccess |> AuthError |> Error |> Task.FromResult
        else task {
            let! customerPreferenceRes = getCustomerPreferences config
            let customerPreference = customerPreferenceRes |> unwrapResult

            let! customerRes = getCustomer config
            let customer, customerDTO = customerRes |> unwrapResult

            let! wallet = getWallet config customer.Id

            let fullCustomer =
                {   Customer = customer
                    IsVerified = false
                    VerificationEvent = None
                    CustomerPreference = customerPreference
                    CustomerTier =  if customerDTO.CustomerTier |> System.String.IsNullOrWhiteSpace then Unassigned
                                    else (getUnionCaseFromString<CustomerTier> customerDTO.CustomerTier).Value
                    Wallet = wallet.PublicPart TestEnv
                }
            return fullCustomer |> Ok
        }
}   

 
// let checkEthNet config (request: SecureVoidRequest) = task { 
//     printfn "checkEthNet() called"

//     return! 
//         if request.Token |> isTokenValid |> not then TokenInvalid |> AuthError |> Error |> Task.FromResult
//         elif request.Token |> checkAuthTokenValid config |> not then UserDoesNotHaveAccess |> AuthError |> Error |> Task.FromResult
//         else task {
//             let! customerRes = getCustomer config
//             let customer, customerDTO = customerRes |> unwrapResult
//             let! wallet = getWallet config customer.Id
//             let transactions = checkETHNet wallet.Main.Eth

//             return transactions |> Ok
//         }
// }   



let  getTransactions config (request: SecureVoidRequest) = task { 
    printfn "getTransactions() called"

    return! 
        if request.Token |> isTokenValid |> not then TokenInvalid |> AuthError |> Error |> Task.FromResult
        elif request.Token |> checkAuthTokenValid config |> not then UserDoesNotHaveAccess |> AuthError |> Error |> Task.FromResult
        else task {
            let! customerRes = getCustomer config
            let customer, customerDTO = customerRes |> unwrapResult
            let! wallet = getWallet config customer.Id
            let checkETHNet = checkETHNet wallet.Main.Eth
            let! transactions = getTransactions wallet.Main.Eth checkETHNet

            return transactions |> Ok
        }
}   

async { while true do 
                    getTransactions 
                    do! Async.Sleep 5000 } |> Async.Start
                    
module PriceUpdater = 
    let [<Literal>] private CCUrl = "https://min-api.cryptocompare.com/data/pricemulti?fsyms=BTC,ETH,BTG,LTC,BCH,DASH,ETC&tsyms=USD,EUR,ETH,BTC"
    type private PriceSource = JsonProvider<CCUrl>

    let private getPriceTick() = async {
        try
            let! prices = PriceSource.AsyncLoad(CCUrl) 
            return {    Prices =
                            [
                                {   Symbol = BTC
                                    CryptoCurrencyName = "Bitcoin"
                                    PriceUsd = prices.Btc.Usd |> decimal
                                    PriceEth = prices.Btc.Eth |> decimal
                                    PriceAt = DateTime.Now }
                                {   Symbol = ETH
                                    CryptoCurrencyName = "Ethereum"
                                    PriceUsd = prices.Eth.Usd |> decimal
                                    PriceEth = prices.Eth.Eth |> decimal
                                    PriceAt = DateTime.Now }
                                {   Symbol = LTC
                                    CryptoCurrencyName = "Litecoin"
                                    PriceUsd = prices.Ltc.Usd |> decimal
                                    PriceEth = prices.Ltc.Eth |> decimal
                                    PriceAt = DateTime.Now }
                                {   Symbol = BCH
                                    CryptoCurrencyName = "Bitcoin Cash"
                                    PriceUsd = prices.Bch.Usd |> decimal
                                    PriceEth = prices.Bch.Eth |> decimal
                                    PriceAt = DateTime.Now }
                                {   Symbol = BTG
                                    CryptoCurrencyName = "Bitcoin Gold"
                                    PriceUsd = prices.Btg.Usd |> decimal
                                    PriceEth = prices.Btg.Eth |> decimal
                                    PriceAt = DateTime.Now }
                                {   Symbol = ETC
                                    CryptoCurrencyName = "Ethereum Classic"
                                    PriceUsd = prices.Etc.Usd |> decimal
                                    PriceEth = prices.Etc.Eth |> decimal
                                    PriceAt = DateTime.Now }
                                {   Symbol = DASH
                                    CryptoCurrencyName = "Dash"
                                    PriceUsd = prices.Dash.Usd |> decimal
                                    PriceEth = prices.Dash.Eth |> decimal
                                    PriceAt = DateTime.Now }
                            ] } |> Some
        with e ->
            printfn "Exception while retrieving price updates: %s" e.Message 
            return None
    }

    type private PriceLoadingMsg =
        | LoadPrices
        | GetPrices of AsyncReplyChannel<ViewModels.CurrencyPriceTick> 

    let private priceLoadingAgent = MailboxProcessor.Start(fun inbox-> 
        let rec messageLoop (prices: CurrencyPriceTick) = async {
            let! msg = inbox.Receive()
            match msg with
            | LoadPrices ->
                let! newPrices = getPriceTick()
                // printfn "Prices loaded"
                match newPrices with
                | Some prices ->
                    bridgeConnections.SendIf(function | Connected _ -> true | Disconnected -> false) 
                        (prices |> ServerPriceTick |> BC |> C)
                | None -> ()
                return! messageLoop (newPrices |> Option.defaultValue prices)
            | GetPrices replyChannel -> 
                replyChannel.Reply prices
                return! messageLoop prices 
        }
        // start the loop 
        messageLoop { Prices = [] } 
    )

    // Starts background price loading
    async { while true do 
                priceLoadingAgent.Post LoadPrices 
                do! Async.Sleep 5000 } |> Async.Start

    let getLatestPrices() = priceLoadingAgent.PostAndAsyncReply (GetPrices, 3000)

let getPriceTick config i = task {
        let! prices = PriceUpdater.getLatestPrices() |> Async.StartAsTask
        return prices |> Ok
}


// let private transactionsAgent = MailboxProcessor.Start(fun inbox-> 
//         let rec messageLoop (prices: ETransactions) = async {
//             let! msg = inbox.Receive()
//             match msg with
//             | LoadTransactions ->
//                 let! newPrices = getTransactionTick()
//                 // printfn "Prices loaded"
//                 match newPrices with
//                 | Some transactions ->
//                     bridgeConnections.SendIf(function | Connected _ -> true | Disconnected -> false) 
//                         (transactions |> ServerTransactionTick |> BC |> C)
//                 | None -> ()
//                 return! messageLoop (newTransactions |> Option.defaultValue transactions)
//             | GetTransactions replyChannel -> 
//                 replyChannel.Reply prices
//                 return! messageLoop prices 
//         }
//         // start the loop 
//         messageLoop { ETransactions = [] } 
//     )
//     let getTransactions1() = transactionsAgent.PostAndAsyncReply (getTransactions, 3000)
// let getTransactionTick config i = task {
//         let! transactions = transactionsAgent config |> Async.StartAsTask
//         return transactions |> Ok
// }

let toClientMsg = BC >> C >> Cmd.ofMsg   

let bridgeInit () =
    printfn "Server init"
    Disconnected, ServerConnected |> toClientMsg

let bridgeUpdate config msg state =
    printfn "bridgeUpdate: %A" msg
    match msg with
    | Closed                -> Disconnected, Cmd.none
    | ConnectUser authToken ->
        if checkAuthTokenValid config authToken then 
            Connected authToken, UserConnected authToken |> toClientMsg
        else state, ErrorResponse(TokenInvalid |> AuthError, msg) |> toClientMsg
    | DisconnectUser        -> Disconnected, Cmd.none

let cabinetProtocol config =
    {   getCryptoCurrencies = getCryptoCurrencies   config    >> Async.AwaitTask
        getTokenSale        = getTokenSale          config    >> Async.AwaitTask
        getFullCustomer     = getFullCustomer       config    >> Async.AwaitTask
        getTransactions     = getTransactions       config    >> Async.AwaitTask
        getPriceTick        = getPriceTick          config    >> Async.AwaitTask
    }
    
let bridgeProtocol config =
    bridge bridgeInit (bridgeUpdate config) {
        serverHub bridgeConnections
        at Shared.Route.wsBridgeEndpoint
    }       
open System.IO
open System.Threading.Tasks
open FSharp.Data

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Saturn
open Config

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open Shared.ViewModels
open Shared.Auth
open Shared.Utils
open Shared.Result
open ServerUtils

open Customer.Wallet
open System
open Microsoft.AspNetCore.Http
open NBitcoin.Protocol
open Dapper

open TypeShape.Tools

open Elmish
open Elmish.Bridge

let publicPath = Path.GetFullPath "../Client/public"

let port = 8085us

let getInitCounter () : Task<ServerResult<Counter>> = task { return Ok 42 }
let initDb () = task {  printfn "\n\ninitDb() called\n\n" 
                        return Ok () }

let createCustomer email password ethAddress : Customers.Customer =
    let id = System.Guid.NewGuid().ToString("N") 
    {   Id              = id
        Email           = email
        FirstName       = ""
        LastName        = ""
        EthAddress      = ethAddress
        Password        = password
        PasswordSalt    = id + email
        Avatar          = "MyPicture"
        CustomerTier    = Tier1.ToString() }


// let private logins = new System.Collections.Concurrent.ConcurrentDictionary<AuthToken, AuthJwt.UserRights>() //TODO: store in the db

let issueAuthToken email =
    let userRigths = { AuthJwt.UserRights.UserName = email }
    userRigths |> AuthJwt.encode |> AuthToken

let saveAuthToken connectionString (authToken: AuthToken) customerId =
    let auth: AuthTokens.AuthToken = {  AuthToken = authToken.Token
                                        CustomerId = customerId
                                        Issued = DateTime.Now
                                        Expires = DateTime.Now.AddMonths 1 }
    AuthTokens.Database.insert connectionString auth |> Ok 

let private isTokenValid authToken =
    let (AuthToken authToken) = authToken
    authToken |> AuthJwt.checkValid |> Option.isSome

let private checkAuthTokenValid config authToken =
    //logins.ContainsKey authToken
    true

let private login config (loginInfo: LoginInfo) : Task<LoginResult> = task {
    let! customerRes = Customers.Database.getByEmail config.connectionString loginInfo.Email
    let r = result {
        let! customerOpt = customerRes |> Result.mapError (LoginInternalError >> LoginServerError)
        let! customer = customerOpt |> Result.ofOption EmailNotFoundOrPasswordIncorrect
        let authToken = issueAuthToken customer.Email
        
        let! _ = saveAuthToken config.connectionString authToken customer.Id 
                    |> Result.mapError (LoginInternalError >> LoginServerError) // TODO: Add reaction to non-zero return code

        return authToken 
    }

    return r |> Ok 
}

let private register config (loginInfo: LoginInfo) : Task<RegisteringResult> = task {  // TODO: Change this!!!
    let authToken = issueAuthToken loginInfo.Email
    let customer = createCustomer loginInfo.Email loginInfo.Password ""
    let! res = Customers.Database.insert config.connectionString customer
    let! authInsertRes = 
        match res with
        | Ok code when code = 0 -> saveAuthToken config.connectionString authToken customer.Id 
        | Ok code -> 
            let msg = sprintf "Error creating new customer. Code: '%d'" code
            printfn "%s" msg
            InternalError (new Exception(msg)) |> Error
        | Error exn -> exn |> InternalError |> Error
        |> Result.reWrap
    return 
        match authInsertRes with 
        | Ok r -> 
            match r with 
            | Ok code when code = 0 -> authToken |> Ok |> Ok 
            | Ok code -> 
                let msg = sprintf "Error when saving authToken, Code: '%d'" code
                printfn "%s" msg
                InternalError (new Exception(msg)) |> Error 
            | Error exn -> exn |> InternalError |> Error 
        | Error exn -> exn |> Error 
}

let private forgotPassword config (forgotPasswordInfo: ForgotPasswordInfo) = task { // TODO: Change this!!!
    return "Reset email sent" |> Ok |> Ok
}

let private resetPassword config resetPasswordInfo = task { // TODO: Change this!!!
    let userRigths = { AuthJwt.UserRights.UserName = "trader@cryptoinvestor.com" }
    let token = userRigths |> AuthJwt.encode |> AuthToken
    // logins.[token] <- userRigths
    return token |> Ok |> Ok
}

module Seed =
    let seedT connectionString deleteAll insert lst = task {
        let! _ = deleteAll connectionString
        for l in lst do
            printfn "Seeding Type: %A" (l.GetType())
            let! res = insert connectionString l
            printfn "Seeding Result: %A" res
    }

    let saleTokenSeed connectionString = 
        let lst: SaleTokens.SaleToken list = 
            [{  Id = "AIM"
                Name = "AIM Network"
                LogoUrl = "assets/AIMLogo.jpg"
                TotalSupply = 100_000_000M }] 
        lst |> seedT connectionString SaleTokens.Database.deleteAll SaleTokens.Database.insert 

    let stageStatusesSeed connectionString = 
        let lst: TokenSaleStageStatuses.TokenSaleStageStatus list = 
            [   {   Id                  = 1
                    TokenSaleStageId    = 1
                    Status              = TokenSaleStageStatus.Completed.ToString()
                    CreatedOn           = DateTime.UtcNow
                    CreatedBy           = "Initial"
                    Proof               = "Initial Proof" }
                {   Id                  = 2
                    TokenSaleStageId    = 2
                    Status              = TokenSaleStageStatus.Active.ToString()
                    CreatedOn           = DateTime.UtcNow
                    CreatedBy           = "Initial"
                    Proof               = "Initial Proof" }
                {   Id                  = 3
                    TokenSaleStageId    = 3
                    Status              = TokenSaleStageStatus.Expectation.ToString()
                    CreatedOn           = DateTime.UtcNow
                    CreatedBy           = "Initial"
                    Proof               = "Initial Proof" }]
        lst |> seedT connectionString TokenSaleStageStatuses.Database.deleteAll TokenSaleStageStatuses.Database.insert  
    let stagesSeed connectionString = 
        let lst: TokenSaleStages.TokenSaleStage list = 
            [   {   Id                  = 1
                    TokenSaleId         = 1
                    Name                = "Private Sale"
                    CapEth              = 300M
                    CapUsd              = 150_000M
                    StartDate           = DateTime.Today
                    EndDate             = DateTime.Today.AddMonths 1 
                    CreatedOn           = DateTime.UtcNow
                    CreatedBy           = "Initial"
                    Proof               = "Initial Proof" }
                {   Id                  = 2
                    TokenSaleId         = 1
                    Name                = "Pre ICO"
                    CapEth              = 1000M
                    CapUsd              = 500_000M
                    StartDate           = DateTime.Today.AddMonths 1
                    EndDate             = DateTime.Today.AddMonths 2 
                    CreatedOn           = DateTime.UtcNow
                    CreatedBy           = "Initial"
                    Proof               = "Initial Proof" }
                {   Id                  = 3
                    TokenSaleId         = 1
                    Name                = "ICO"
                    CapEth              = 30000M
                    CapUsd              = 15_000_000M
                    StartDate           = DateTime.Today.AddMonths 2
                    EndDate             = DateTime.Today.AddMonths 3 
                    CreatedOn           = DateTime.UtcNow
                    CreatedBy           = "Initial"
                    Proof               = "Initial Proof" } ]
        lst |> seedT connectionString TokenSaleStages.Database.deleteAll TokenSaleStages.Database.insert

    let tokenSaleStatusSeed connectionString startDate endDate = 
        let lst: TokenSaleStatuses.TokenSaleStatus list = 
            [   {   Id                  = 1
                    TokenSaleId         = 1
                    TokenSaleStatus     = TokenSaleStatus.Active.ToString()
                    ActiveStageId       = 2
                    SaleTokenId         = 1
                    PriceUsd            = 5M
                    PriceEth            = 0.01M
                    BonusPercent        = 20M
                    BonusTokens         = 100M
                    StartDate           = startDate
                    EndDate             = endDate 
                    CreatedOn           = DateTime.UtcNow
                    CreatedBy           = "Initial"
                    Proof               = "Initial Proof" } ]
        lst |> seedT connectionString TokenSaleStatuses.Database.deleteAll TokenSaleStatuses.Database.insert

    let tokenSaleSeed connectionString startDate endDate = 
        let lst: TokenSales.TokenSale list = 
            [   {   Id          = 1
                    Symbol      = "AIM"
                    SoftCapEth  = 10_000_000M
                    HardCapEth  = 50_000_000M
                    SoftCapUsd  = 10_000_000M
                    HardCapUsd  = 50_000_000M
                    Expectations = 50_000_000M
                    StartDate   = startDate
                    EndDate     = endDate
                    CreatedOn   = DateTime.UtcNow
                    CreatedBy   = "Initial"
                    Proof       = "Initial Proof" } ]
        lst |> seedT connectionString TokenSales.Database.deleteAll TokenSales.Database.insert

    let customerPreferencesSeed connectionString =
        let lst: CustomerPreferences.CustomerPreference list = 
            [   {   Id          = Guid.NewGuid().ToString("N")
                    Language    = CustomerPreferences.Validation.supportedLangs.[0] } ]
        lst |> seedT connectionString CustomerPreferences.Database.deleteAll CustomerPreferences.Database.insert       

    let customerSeed connectionString =
        let lst: Customers.Customer list = 
            [ createCustomer "trader@cryptoinvestor.com" "!!!ChangeMe111" "0x001002003004005006007008009" ]
        lst |> seedT connectionString Customers.Database.deleteAll Customers.Database.insert        

    let walletsSeed connectionString = task {
        let! _ = WalletsKV.Database.deleteAll connectionString
        ()
    }

    let authTokenSeed connectionString = task {
        let! _ = AuthTokens.Database.deleteAll connectionString
        ()
    }
    

    let seedAll connectionString = task {
        do! saleTokenSeed connectionString
        printfn "Seeding ..."
        do! stageStatusesSeed connectionString
        do! stagesSeed connectionString
        let startDate = DateTime.Today
        let endDate   = DateTime.Today.AddMonths 3
        do! tokenSaleStatusSeed connectionString startDate endDate
        do! tokenSaleSeed connectionString startDate endDate

        do! customerPreferencesSeed connectionString
        do! customerSeed connectionString

        do! walletsSeed connectionString
        do! authTokenSeed connectionString
    }

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
            Password    = customer.Password
            PasswordSalt = customer.PasswordSalt
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
                printfn "Prices loaded"
                match newPrices with
                | Some prices ->
                    bridgeConnections.SendIf(function | Connected _ -> true | Disconnected -> false) (prices |> WsBridge.ServerPriceTick |> C)
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

type CustomError = { errorMsg: string }

let errorHandler (ex: Exception) (routeInfo: RouteInfo<HttpContext>) = 
    // do some logging
    printfn "Error at %s on method %s" routeInfo.path routeInfo.methodName
    printfn "Exception %A" ex 
    // decide whether or not you want to propagate the error to the client
    match ex with
    | :? IOException as x ->
        // propagate custom error, this is intercepted by the client
        let customError = { errorMsg = "Something terrible happend" }
        Propagate customError
    | _ ->
        // ignore error
        Ignore



open Shared.WsBridge

let webApp config =
    // Setting up remoting
    let loginProtocol =
        {   login               = login                 config    >> Async.AwaitTask
            register            = register              config    >> Async.AwaitTask
            forgotPassword      = forgotPassword        config    >> Async.AwaitTask
            resetPassword       = resetPassword         config    >> Async.AwaitTask
        }            
    let tokenSaleProtocol =
        {   getCryptoCurrencies = getCryptoCurrencies   config    >> Async.AwaitTask
            getTokenSale        = getTokenSale          config    >> Async.AwaitTask
            getFullCustomer     = getFullCustomer       config    >> Async.AwaitTask
            getPriceTick        = getPriceTick          config    >> Async.AwaitTask
        }

   
    let bridgeInit () =
        printfn "Server init"
        Disconnected, Cmd.ofMsg (C ServerConnected)

    let bridgeUpdate config msg state =
        printfn "bridgeUpdate: %A" msg
        match msg with
        | Closed                -> Disconnected, Cmd.none
        | ConnectUser authToken ->
            if checkAuthTokenValid config authToken then 
                Connected authToken, UserConnected authToken |> C |> Cmd.ofMsg
            else state, ErrorResponse(TokenInvalid |> AuthError, msg) |> C |> Cmd.ofMsg
        | DisconnectUser        -> Disconnected, Cmd.none
        
    let bridgeProtocol =
        bridge bridgeInit (bridgeUpdate config) {
            serverHub bridgeConnections
            at Shared.Route.wsBridgeEndpoint
        }        
        
    choose [
        remoting loginProtocol {
            use_route_builder Route.builder
            use_error_handler errorHandler
        }        
        remoting tokenSaleProtocol {
            use_route_builder Route.builder
            use_error_handler errorHandler
        }
        bridgeProtocol
        Router.router
    ]

let app config = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    router (webApp config)
    app_config Giraffe.useWebSockets
    memory_cache
    use_static publicPath
    use_gzip
    use_config (fun _ -> config)
}

let config = { connectionString = "DataSource=database.sqlite" }

// Seeding
try
    printfn "Seeding 1..."
    Seed.seedAll config.connectionString
    |> Async.AwaitTask 
    |> Async.RunSynchronously

    run (app config)
with 
| e -> printfn "SEEDING ERROR: %A" e



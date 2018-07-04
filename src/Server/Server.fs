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

open Customer.Wallet
open System
open Microsoft.AspNetCore.Http

let publicPath = Path.GetFullPath "../Client/public"

let port = 8085us

let getInitCounter () : Task<ServerResult<Counter>> = task { return Ok 42 }
let initDb () = task {  printfn "\n\ninitDb() called\n\n" 
                        return Ok () }

// let private syncRoot = new obj()
// let get

let private logins = new System.Collections.Concurrent.ConcurrentDictionary<AuthToken, AuthJwt.UserRights>() //TODO: store in the db

let private login config (loginInfo: LoginInfo) = task { 
    let userRigths = { AuthJwt.UserRights.UserName = loginInfo.Email }
    let token = userRigths |> AuthJwt.encode |> AuthToken
    logins.[token] <- userRigths
    return token |> Ok |> Ok
}

let private register config (loginInfo: LoginInfo) = task {  // TODO: Change this!!!
    let userRigths = { AuthJwt.UserRights.UserName = loginInfo.Email }
    let token = userRigths |> AuthJwt.encode |> AuthToken
    logins.[token] <- userRigths
    return token |> Ok |> Ok
}

let private forgotPassword config (forgotPasswordInfo: ForgotPasswordInfo) = task { // TODO: Change this!!!
    return "Reset email sent" |> Ok |> Ok
}

let private resetPassword config resetPasswordInfo = task { // TODO: Change this!!!
    let userRigths = { AuthJwt.UserRights.UserName = "trader@cryptoinvestor.com" }
    let token = userRigths |> AuthJwt.encode |> AuthToken
    logins.[token] <- userRigths
    return token |> Ok |> Ok
}

let private checkUserExists authToken =
    logins.ContainsKey authToken

let private isTokenValid authToken =
    let (AuthToken authToken) = authToken
    authToken |> AuthJwt.checkValid |> Option.isSome

module Seed =
    // let seed config deleteAll lst = task {
    //     let cryptoCurrencies = getUnionCaseNames<CryptoCurrencySymbol> |> List.map (fun cc -> { CryptoCurrencySymbol.Id = cc; Name = cc; LogoUrl = ""; UpdateUrl = "" })
    //     let! _ = CryptoCurrencies.Database.deleteAll config
    //     for cc in cryptoCurrencies do
    //         let! _ = CryptoCurrencies.Database.insert config cc
    //         ()
    // }

    let seedT config deleteAll insert lst = task {
        let! _ = deleteAll config
        for l in lst do
            let! _ = insert config l
            ()
    }

    let saleTokenSeed config = [{   Symbol = "AIM"
                                    Name = "AIM Network"
                                    LogoUrl = "assets/AIMLogo.jpg"
                                    TotalSupply = 100_000_000M }] 
                                |> seedT config SaleTokens.Database.deleteAll SaleTokens.Database.insert 

    let stageStatusesSeed config = [   {    Id = 1
                                            Name = "Private Sale"
                                            CapEth = 300M
                                            CapUsd = 150_000M
                                            StartDate = DateTime.Today
                                            EndDate = DateTime.Today.AddMonths 1
                                            Status = TokenSaleStageStatus.Completed }
                                        {   Id = 2
                                            Name = "Pre ICO"
                                            CapEth = 1000M
                                            CapUsd = 500_000M
                                            StartDate = DateTime.Today.AddMonths 1
                                            EndDate = DateTime.Today.AddMonths 2
                                            Status = TokenSaleStageStatus.Active }

                                        {   Id = 3
                                            Name = "ICO"
                                            CapEth = 30000M
                                            CapUsd = 15_000_000M
                                            StartDate = DateTime.Today.AddMonths 2
                                            EndDate = DateTime.Today.AddMonths 3
                                            Status = TokenSaleStageStatus.Expectation } ]
                                    |> seedT config TokenSaleStageStatuses.Database.deleteAll TokenSaleStageStatuses.Database.insert  
    let stagesSeed config = [   {   Id = 1
                                    Name = "Private Sale"
                                    CapEth = 300M
                                    CapUsd = 150_000M
                                    StartDate = DateTime.Today
                                    EndDate = DateTime.Today.AddMonths 1
                                    Status = TokenSaleStageStatus.Completed }
                                {   Id = 2
                                    Name = "Pre ICO"
                                    CapEth = 1000M
                                    CapUsd = 500_000M
                                    StartDate = DateTime.Today.AddMonths 1
                                    EndDate = DateTime.Today.AddMonths 2
                                    Status = TokenSaleStageStatus.Active }

                                {   Id = 3
                                    Name = "ICO"
                                    CapEth = 30000M
                                    CapUsd = 15_000_000M
                                    StartDate = DateTime.Today.AddMonths 2
                                    EndDate = DateTime.Today.AddMonths 3
                                    Status = TokenSaleStageStatus.Expectation } ]
                            |> seedT config TokenSaleStages.Database.deleteAll TokenSaleStages.Database.insert                             

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
    let! res = getAll(config.connectionString) 
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

let getTokenSaleStages config = task {  
    let processStage (stage: TokenSaleStages.TokenSaleStage) = task {
        let! statusRes = TokenSaleStageStatuses.Database.getByStageId config.connectionString stage.Id
        let status = match statusRes with 
                        | Ok (Some status) -> status.Status
                        | Ok None -> failwithf "Incorrect Database setup for TokenSaleStage %d Status table" stage.Id
                        | Error exn -> raise exn
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


let getTokenSale config () = task { 
    printfn "getTokenSale() called"

    let saleToken = getSaleToken config


    let tokenSaleState: TokenSaleState = {  TokenSaleStatus = TokenSaleStatus.Active
                                            ActiveStage = preICOStage
                                            PriceUsd = 5M
                                            PriceEth = 0.01M
                                            BonusPercent = 20M
                                            BonusTokens = 100M
                                            StartDate = privateSaleStage.StartDate
                                            EndDate = ICOStage.EndDate }

    let tokenSale: TokenSale =          {   SaleToken   = saleToken
                                            SoftCapEth  = 10_000_000M
                                            HardCapEth  = 50_000_000M
                                            SoftCapUsd  = 10_000_000M
                                            HardCapUsd  = 50_000_000M
                                            Expectations = 50_000_000M
                                            StartDate   = DateTime.Today
                                            EndDate     = DateTime.Today.AddMonths 3
                                            
                                            TokenSaleState = tokenSaleState

                                            TokenSaleStages = [ privateSaleStage; preICOStage; ICOStage ] }

    return tokenSale |> Ok
}                                


let  getFullCustomer config (request: SecureVoidRequest) = task { 
    printfn "getFullCustomer() called"

    return 
        if request.Token |> isTokenValid |> not then TokenInvalid |> AuthError |> Error
        elif request.Token |> checkUserExists |> not then UserDoesNotHaveAccess |> AuthError |> Error
        else 
            let customer: Customer = 
                {   Id = System.Guid.NewGuid()
                    FirstName = "John"
                    LastName = "Smith"
                    EthAddress = "0x001002003004005006007008009"
                    Password = "!!!ChangeMe!!!"
                    PasswordSalt = "!!PwdSalt!!"
                    Avatar = "MyPicture"
                    Email = "trader@cryptoinvestor.com"
                }

            let customerPreference: CustomerPreference = 
                {   CustomerId = customer.Id
                    Language   = CustomerPreferences.Validation.supportedLangs.[0] }

            let fullCustomer =
                {   Customer = customer
                    IsVerified = false
                    VerificationEvent = None
                    CustomerPreference = customerPreference
                    CustomerTier = Tier1
                    Wallet = (createCustomerWallet customer.Id).PublicPart TestEnv
                }
            fullCustomer |> Ok
}   

module PriceUpdater = 
    let [<Literal>] private CCUrl = "https://min-api.cryptocompare.com/data/pricemulti?fsyms=BTC,ETH,BTG,LTC,BCH,DASH,ETC&tsyms=USD,EUR,ETH,BTC"
    type private PriceSource = JsonProvider<CCUrl>

    let private getPriceTick() = async {
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
                        ] }
    }

    type private PriceLoadingMsg =
        | LoadPrices
        | GetPrices of AsyncReplyChannel<ViewModels.CurrencyPriceTick> 

    let private priceLoadingAgent = MailboxProcessor.Start(fun inbox-> 
        let rec messageLoop prices = async {
            let! msg = inbox.Receive()
            match msg with
            | LoadPrices ->
                let! prices = getPriceTick()
                printfn "Prices loaded"
                return! messageLoop prices
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

let webApp config =
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
    let adminProtocol =
        {   getInitCounter  = getInitCounter    >> Async.AwaitTask 
            initDb          = initDb            >> Async.AwaitTask }
        
        
    choose [
        remoting loginProtocol {
            use_route_builder Route.builder
            use_error_handler errorHandler
        }        
        remoting tokenSaleProtocol {
            use_route_builder Route.builder
            use_error_handler errorHandler
        }
        remoting adminProtocol {
            use_route_builder Route.builder
            use_error_handler errorHandler
        }
        Router.router
    ]

let app config = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    router (webApp config)
    memory_cache
    use_static publicPath
    use_gzip
    use_config (fun _ -> config)
}

let config = { connectionString = "DataSource=database.sqlite" }

run (app config)
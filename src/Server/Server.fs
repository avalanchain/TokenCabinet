open System.IO
open System.Threading.Tasks
open FSharp.Data

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Saturn
open Config
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Shared.ViewModels
open TokenSaleStageStatuses

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let getInitCounter () : Task<ServerResult<Counter>> = task { return Ok 42 }
let initDb () = task {  printfn "\n\ninitDb() called\n\n" 
                        return Ok () }

let getCryptoCurrencies config () = task { 
    printfn "getCryptoCurrencies() called"
    let! res = CryptoCurrencies.Database.getAll(config.connectionString) 
    return match res with
            | Ok o -> o |> Seq.toList |> Ok
            | Error exn ->  printfn "Data access exception: '%A'" exn
                            exn |> InternalError |> Error
}

let getTokenSale config () = task { 
    printfn "getTokenSale() called"

    let saleToken: SaleToken = {    Symbol = "AIM"
                                    Name = "AIM Network"
                                    LogoUrl = "assets/AIMLogo.jpg"
                                    TotalSupply = 100_000_000M }

    let privateSaleStage: TokenSaleStage = {Id = 1
                                            Name = "Private Sale"
                                            CapEth = 300M
                                            CapUsd = 150_000M
                                            StartDate = System.DateTime.Today
                                            EndDate = System.DateTime.Today.AddMonths 1
                                            Status = TokenSaleStageStatus.Completed }

    let preICOStage: TokenSaleStage =   {   Id = 2
                                            Name = "Pre ICO"
                                            CapEth = 1000M
                                            CapUsd = 500_000M
                                            StartDate = System.DateTime.Today.AddMonths 1
                                            EndDate = System.DateTime.Today.AddMonths 2
                                            Status = TokenSaleStageStatus.Active }

    let ICOStage: TokenSaleStage =      {   Id = 3
                                            Name = "ICO"
                                            CapEth = 30000M
                                            CapUsd = 15_000_000M
                                            StartDate = System.DateTime.Today.AddMonths 2
                                            EndDate = System.DateTime.Today.AddMonths 3
                                            Status = TokenSaleStageStatus.Expectation }

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
                                            StartDate   = System.DateTime.Today
                                            EndDate     = System.DateTime.Today.AddMonths 3
                                            
                                            TokenSaleState = tokenSaleState

                                            TokenSaleStages = [ privateSaleStage; preICOStage; ICOStage ] }

    return tokenSale |> Ok
}                                

let getFullCustomer config () = task { 
    printfn "getFullCustomer() called"
    
    let customer: Customers.Customer = 
        {   Id = System.Guid.NewGuid()
            FirstName = "John"
            LastName = "Smith"
            EthAddress = "0x001002003004005006007008009"
            Password = "!!!ChangeMe!!!"
            PasswordSalt = "!!PwdSalt!!"
            Avatar = "MyPicture"
        }

    let customerPreference: CustomerPreferences.CustomerPreference = 
        {   Id = customer.Id
            Language = CustomerPreferences.Validation.supportedLangs.[0] }

    let fullCustomer =
        {   Customer = customer
            IsVerified = false
            VerificationEvent = None
            CustomerPreference = customerPreference
        }
    return fullCustomer |> Ok
}   

module PriceUpdater = 
    let [<Literal>] private CCUrl = "https://min-api.cryptocompare.com/data/pricemulti?fsyms=BTC,ETH,BTG,LTC,BCH,DASH,ETC&tsyms=USD,EUR,ETH,BTC"
    type private PriceSource = JsonProvider<CCUrl>

    let private getPriceTick() = async {
        let! prices = PriceSource.AsyncLoad(CCUrl) 
        return {    Prices =
                        [
                            {   Symbol = "BTC"
                                CryptoCurrencyName = "Bitcoin"
                                PriceUsd = prices.Btc.Usd |> decimal
                                PriceEth = prices.Btc.Eth |> decimal
                                PriceAt = System.DateTime.Now }
                            {   Symbol = "ETH"
                                CryptoCurrencyName = "Ethereum"
                                PriceUsd = prices.Ltc.Usd |> decimal
                                PriceEth = prices.Ltc.Eth |> decimal
                                PriceAt = System.DateTime.Now }
                            {   Symbol = "LTC"
                                CryptoCurrencyName = "Litecoin"
                                PriceUsd = prices.Ltc.Usd |> decimal
                                PriceEth = prices.Ltc.Eth |> decimal
                                PriceAt = System.DateTime.Now }
                            {   Symbol = "BCH"
                                CryptoCurrencyName = "Bitcoin Cash"
                                PriceUsd = prices.Bch.Usd |> decimal
                                PriceEth = prices.Bch.Eth |> decimal
                                PriceAt = System.DateTime.Now }
                            {   Symbol = "BTG"
                                CryptoCurrencyName = "Bitcoin Gold"
                                PriceUsd = prices.Btg.Usd |> decimal
                                PriceEth = prices.Btg.Eth |> decimal
                                PriceAt = System.DateTime.Now }
                            {   Symbol = "ETC"
                                CryptoCurrencyName = "Ethereum Classic"
                                PriceUsd = prices.Etc.Usd |> decimal
                                PriceEth = prices.Etc.Eth |> decimal
                                PriceAt = System.DateTime.Now }
                            {   Symbol = "DASH"
                                CryptoCurrencyName = "Dash"
                                PriceUsd = prices.Dash.Usd |> decimal
                                PriceEth = prices.Dash.Eth |> decimal
                                PriceAt = System.DateTime.Now }
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

let getPriceTick2 config i = task {
    return {    Prices =
                    [
                        {   Symbol = "BTC"
                            CryptoCurrencyName = "Bitcoin"
                            PriceUsd = 7000UL + i |> decimal
                            PriceEth = 15UL + i |> decimal
                            PriceAt = System.DateTime.Now }
                        {   Symbol = "ETH"
                            CryptoCurrencyName = "Ethereum"
                            PriceUsd = 500UL + i |> decimal
                            PriceEth = 1UL + i |> decimal
                            PriceAt = System.DateTime.Now }
                    ] }
        |> Ok
}

let webApp config =
    let adminProtocol =
        {   getInitCounter  = getInitCounter    >> Async.AwaitTask 
            initDb          = initDb            >> Async.AwaitTask }
    let tokenSaleProtocol =
        {   getCryptoCurrencies = getCryptoCurrencies   config    >> Async.AwaitTask
            getTokenSale        = getTokenSale          config    >> Async.AwaitTask
            getFullCustomer     = getFullCustomer       config    >> Async.AwaitTask
            getPriceTick        = getPriceTick          config    >> Async.AwaitTask
        }
        
    choose [
        remoting adminProtocol {
            use_route_builder Route.builder
        }
        remoting tokenSaleProtocol {
            use_route_builder Route.builder
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
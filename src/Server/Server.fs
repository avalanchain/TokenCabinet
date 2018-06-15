open System.IO
open System.Threading.Tasks

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

open CryptoCurrencyPrices
let getPriceTick config i = task {
    return 
        [
            "BTC", {    CryptoCurrencyPrice.Id = 1
                        CryptoCurrencyName = "Bitcoin"
                        PriceUsd = 7000UL + i |> decimal
                        PriceEth = 15UL + i |> decimal
                        PriceAt = System.DateTime.Now
                        CreatedOn = System.DateTime.Now
                        CreatedBy = System.DateTime.Now // TODO: Fix type error
                        Proof = "ALL_GOOD" }
            "ETH", {    CryptoCurrencyPrice.Id = 2
                        CryptoCurrencyName = "Ethereum"
                        PriceUsd = 500UL + i |> decimal
                        PriceEth = 1UL + i |> decimal
                        PriceAt = System.DateTime.Now
                        CreatedOn = System.DateTime.Now
                        CreatedBy = System.DateTime.Now // TODO: Fix type error
                        Proof = "ALL_GOOD" }
        ]
        |> Map.ofList
        |> ViewModels.CurrencyPriceTick
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
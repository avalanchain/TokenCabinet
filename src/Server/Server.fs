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

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let getInitCounter () : Task<ServerResult<Counter>> = task { return Ok 42 }
let initDb () = task { printfn "\n\ninitDb() called\n\n" 
                }

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

    // let saleToken: SaleTokens.SaleToken = { Id = "AIM"
    //                                         Name = "AIM Network"
    //                                         LogoUrl = "assets/AIMLogo.jpg"
    //                                         UpdateUrl = "NOT_USED" }

    // let tokenSale = {
    //     Id = 1
    //     SaleToken = saleToken
    //     SoftCapEth = 10_000_000M
    //     HardCapEth = 50_000_000M
    //     SoftCapUsd = 10_000_000M
    //     HardCapUsd = 50_000_000M
    //     Expectations: decimal
    //     StartDate: System.DateTime
    //     EndDate: System.DateTime
        
    //     TokenSaleStatus: TokenSaleStatus

    //     TokenSaleStages: Set<TokenSaleStage>
    //     TokenSaleStatusIds: Set<TokenSaleStatusIds.TokenSaleStatusId>
    //     TokenSaleStageStatusIds: Set<TokenSaleStageStatusIds.TokenSaleStageStatusId>
    // }

    return NotImplementedError |> Error
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


let webApp config =
    let adminProtocol =
        {   getInitCounter  = getInitCounter    >> Async.AwaitTask 
            initDb          = initDb            >> Async.AwaitTask }
    let tokenSaleProtocol =
        {   getCryptoCurrencies = getCryptoCurrencies   config    >> Async.AwaitTask
            getTokenSale        = getTokenSale          config    >> Async.AwaitTask
            getFullCustomer     = getFullCustomer       config    >> Async.AwaitTask
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
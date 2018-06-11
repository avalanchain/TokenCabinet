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

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let getInitCounter () : Task<Counter> = task { return 42 }
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


let webApp config =
    let adminProtocol =
        {   getInitCounter  = getInitCounter    >> Async.AwaitTask 
            initDb          = initDb            >> Async.AwaitTask }
    let tokenSaleProtocol =
        {   getCryptoCurrencies = getCryptoCurrencies config >> Async.AwaitTask
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
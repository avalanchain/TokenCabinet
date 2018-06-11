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

let webApp =
    let server =
        { getInitCounter = getInitCounter >> Async.AwaitTask }
        
    choose [
        remoting server {
            use_route_builder Route.builder
        }
        Router.router
    ]

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    router webApp
    memory_cache
    use_static publicPath
    use_gzip
    use_config (fun _ -> {connectionString = "DataSource=database.sqlite"})
}

run app
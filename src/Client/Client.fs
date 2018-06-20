module Client.Main

open Elmish
open Elmish.React
open Elmish.Toastr

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Shared

open Fulma

open Fulma.FontAwesome
open Fable
open Fable.Core
open Fable.Import.RemoteDev
open Fable.Import.Browser
open Fable.Import
open JsInterop

open web3Impl
open Fable.Core.JsInterop

open Client
open ClientModelMsg
open System.ComponentModel
// importAll "../../node_modules/bulma/bulma.sass"
importAll "../../node_modules/bulma-steps/dist/css/bulma-steps.min.css"
importAll "../Client/lib/css/dashboard.css"
// importAll "../Client/lib/js/dashboard.js"
let ethHost = match Utils.load<string> "EthereumHost" with
                | Some eh -> eh
                | None -> 
                    let defaultHost = "http://127.0.0.1:8545"
                    Utils.save "EthereumHost" defaultHost
                    defaultHost

let web3: Web3Impl = !!createNew web3 (createNew httpProvider ethHost)
Browser.console.log web3

// open W3
// let w3 = !!createNew W3 ()
// Browser.console.log "W3"
// Browser.console.log w3

//let W3 = importDefault<Web3> "soltsice"
// console.log("W3: " + (string W3))
// let web3 = W3.web3
// console.log("web3: " + (string W3.web3))

module Server =

    open Shared
    open Fable.Remoting.Client

    /// A proxy you can use to talk to server directly
    let adminApi : IAdminProtocol =
        Proxy.remoting<IAdminProtocol> {
            use_route_builder Route.builder
        }

    let tokenSaleApi : ITokenSaleProtocol =
        Proxy.remoting<ITokenSaleProtocol> {
            use_route_builder Route.builder
        }        

let cmdServerCall apiFunc args (completeMsg: 'T -> Msg) serverMethodName =
    Cmd.ofAsync
        apiFunc
        args
        (fun res -> match res with
                    | Ok cc -> cc |> completeMsg
                    | Error serverError -> serverError |> ServerError |> ServerErrorMsg
                    )
        (fun exn -> console.error(sprintf "Exception during %s call: '%A'" serverMethodName exn)
                    exn |> CommunicationError |> ServerErrorMsg)

let init () : Model * Cmd<Msg> =
    let model = {   Auth = None
                    Counter = None
                    CryptoCurrencies = []
                    CurrenciesCurentPrices = { Prices = [] }
                    TokenSale = None
                    MenuMediator = PurchaseToken 
                }
    let cmdInitCounter          = cmdServerCall (Server.adminApi.getInitCounter) () Init "getInitCounter()"
    let cmdGetCryptoCurrencies  = cmdServerCall (Server.tokenSaleApi.getCryptoCurrencies) () GetCryptoCurrenciesCompleted "getCryptoCurrencies()"
    let cmdGetTokenSale         = cmdServerCall (Server.tokenSaleApi.getTokenSale) () GetTokenSaleCompleted "getTokenSale()"

    model, (Cmd.batch [cmdInitCounter; cmdGetCryptoCurrencies; cmdGetTokenSale; Cmd.ofMsg (Tick 0UL) ])

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    console.log(sprintf "Msg: '%A', Model: '%A'" msg model)
    let (model', cmd') : Model * Cmd<Msg> =  
        match model, msg with
        | { Counter = None }  , Init x      -> { model with Counter = Some x }      , Cmd.none
        | { Counter = Some x }, Increment   -> { model with Counter = Some (x + 1) }, Cmd.none
        | { Counter = Some x }, Decrement   -> { model with Counter = Some (x - 1) }, Cmd.none

        | model, InitDb -> model, cmdServerCall (Server.adminApi.initDb) () InitDbCompleted "InitDb()"
        | model, InitDbCompleted(_) -> { model with Counter = Some (100) } , Cmd.none

        | model, GetCryptoCurrenciesCompleted cc ->  { model with Counter = Some (cc.Length); CryptoCurrencies = cc } , Cmd.none
        | model, GetTokenSaleCompleted tc -> { model with TokenSale = Some (tc) } , Cmd.none

        | model, MenuSelected mm -> 
            let cmd =   Toastr.message (sprintf "Menu selected: '%A'" mm)
                        |> Toastr.withProgressBar
                        |> Toastr.position BottomRight
                        |> Toastr.timeout 1000
                        |> Toastr.success
            { model with MenuMediator = mm } , cmd  

        | model, Tick i -> model, cmdServerCall (Server.tokenSaleApi.getPriceTick) i PriceTick "getPriceTick()"
        | model, PriceTick tick -> { model with CurrenciesCurentPrices = tick }, Cmd.none

        | model, ServerErrorMsg serverError -> 
            console.error(sprintf "Server error '%A'" serverError)
            model, Cmd.none
        | model, ErrorMsg(m, msg) -> 
            console.error(sprintf "Unhandled Msg '%A' on Model '%A'" msg m)
            model, Cmd.none
        | model, msg -> model, (model, msg) |> ErrorMsg |> Cmd.ofMsg // Catch all for all messages
    model', cmd'



        //   Control.p [ Control.IsExpanded ]
        //     [ Input.text
        //         [ Input.Disabled true
        //           Input.Value (show model.Counter) ] ]

        //   Control.p [ ]
        //     [ Button.a
        //         [ Button.Color IsInfo
        //           Button.OnClick (fun _ -> dispatch Increment) ]
        //         [ str "+" ] ]


        //   Control.p [ ]
        //     [ Button.a
        //         [ Button.Color IsInfo
        //           Button.OnClick (fun _ -> dispatch Decrement) ]
        //         [ str "-" ] ] 


let view (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [ 
        //   NavBrand.navBrand model dispatch
          LeftMenu.LeftMenu model dispatch
        //   ChildMenu.childMenu model dispatch
          TopNavbar.navBar model dispatch
        //   Container.container [ ]
          div[ Id "dashboard-wrapper"
               Class "columns"]
              [ div [ Class "columns"]
                    [ div [ Class "column"] 
                          []
                      div [ Class "content column is-11"] 
                          [ContentView.contentView model dispatch]
                       ]]
          Footer.footer             
                         ]

let timer initial =
    let sub dispatch = 
        let mutable i = 0UL
        window.setInterval((fun _ ->    i <- i + 1UL
                                        i |> Tick |> dispatch)
                                    , 2000) |> ignore
    Cmd.ofSub sub

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
// |> Program.withSubscription timer
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

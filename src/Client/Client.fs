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
open Fable.PowerPack
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

module LocalStorage = 
    let loadUser () : Auth.AuthToken option =
        BrowserLocalStorage.load "user"

    let saveUserCmd user =
        Cmd.ofFunc (BrowserLocalStorage.save "user") user (fun _ -> LoggedIn user |> AuthMsg) (BrowserStorageFailure >> UnexpectedMsg)

    let deleteUserCmd =
        Cmd.ofFunc BrowserLocalStorage.delete "user" (fun _ -> LoggedOut |> AuthMsg) (BrowserStorageFailure >> UnexpectedMsg)

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
                    | Error serverError -> serverError |> ServerError |> ServerErrorMsg |> UnexpectedMsg
                    )
        (fun exn -> console.error(sprintf "Exception during %s call: '%A'" serverMethodName exn)
                    exn |> CommunicationError |> ServerErrorMsg |> UnexpectedMsg)

let init () : Model * Cmd<Msg> =
    let model = {   Auth = None
                    Counter = None
                    CryptoCurrencies = []
                    CurrenciesCurentPrices = { Prices = [] }
                    TokenSale = None
                    MenuMediator = PurchaseToken 
                }
    let cmdInitCounter          = cmdServerCall (Server.adminApi.getInitCounter) () (Init >> OldMsg) "getInitCounter()"
    let cmdGetCryptoCurrencies  = cmdServerCall (Server.tokenSaleApi.getCryptoCurrencies) () (GetCryptoCurrenciesCompleted >> ServerMsg) "getCryptoCurrencies()"
    let cmdGetTokenSale         = cmdServerCall (Server.tokenSaleApi.getTokenSale) () (GetTokenSaleCompleted >> ServerMsg) "getTokenSale()"
    let cmdTick                 = Cmd.ofMsg (Tick 0UL |> UIMsg)

    model, (Cmd.batch [cmdInitCounter; cmdGetCryptoCurrencies; cmdGetTokenSale; cmdTick ])

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    console.log(sprintf "Msg: '%A', Model: '%A'" msg model)
    let (model', cmd') : Model * Cmd<Msg> =  
        match msg with
        | OldMsg msg_ -> 
            match model, msg_ with
            | { Counter = None }  , Init x      -> { model with Counter = Some x }      , Cmd.none
            | { Counter = Some x }, Increment   -> { model with Counter = Some (x + 1) }, Cmd.none
            | { Counter = Some x }, Decrement   -> { model with Counter = Some (x - 1) }, Cmd.none
            | model, InitDb -> model, cmdServerCall (Server.adminApi.initDb) () (InitDbCompleted >> OldMsg) "InitDb()"
            | model, InitDbCompleted(_) -> { model with Counter = Some (100) } , Cmd.none
            | _ -> model, ("Unhandled", msg, model) |> ErrorMsg |> Cmd.ofMsg // Catch all for all messages

        | AuthMsg(LoggedIn authToken) -> { model with Auth = Some { Token = authToken } } , Cmd.none
        | AuthMsg(LoggedOut)          -> { model with Auth = None } , Cmd.none

        | ServerMsg msg_ ->
            match msg_ with
            | GetCryptoCurrenciesCompleted cc   -> { model with Counter = Some (cc.Length); CryptoCurrencies = cc } , Cmd.none
            | GetTokenSaleCompleted tc          -> { model with TokenSale = Some (tc) } , Cmd.none
            | PriceTick tick                    -> { model with CurrenciesCurentPrices = tick }, Cmd.none

        | UIMsg(MenuSelected mm) -> 
            let cmd =   Toastr.message (sprintf "Menu selected: '%A'" mm)
                        |> Toastr.withProgressBar
                        |> Toastr.position BottomRight
                        |> Toastr.timeout 1000
                        |> Toastr.success
            { model with MenuMediator = mm } , cmd  
        | UIMsg(Tick i) -> model, cmdServerCall (Server.tokenSaleApi.getPriceTick) i (PriceTick >> ServerMsg) "getPriceTick()"

        | UnexpectedMsg msg_ ->
            match msg_ with
            | BrowserStorageFailure _ -> 
                model, ("Browser storage access failed with", msg, model) |> ErrorMsg |> Cmd.ofMsg
            | ServerErrorMsg _ -> 
                model, ("Server error ", msg, model) |> ErrorMsg |> Cmd.ofMsg
        | ErrorMsg(text, msg, m) -> 
            console.error(sprintf "%s Msg '%A' on Model '%A'" text msg m)
            model, Cmd.none
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
          LeftMenu.LeftMenu model (UIMsg >> dispatch)
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
                                        i |> Tick |> UIMsg |> dispatch)
                                    , 2000) |> ignore
    Cmd.ofSub sub

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withSubscription timer
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

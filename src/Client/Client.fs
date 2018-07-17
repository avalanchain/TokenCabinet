module Client.Main

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.React
open Elmish.Toastr
open Elmish.Bridge
open Elmish.Bridge.Browser

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Shared

open Fable
open Fable.Core
open Fable.Import.RemoteDev
open Fable.Import.Browser
open Fable.Import
open JsInterop

open Fable.Core.JsInterop

open Client
open Client.Page
open ClientMsgs
open ClientModels
open CabinetModel
open System.ComponentModel
open Fable.PowerPack
open Shared.Utils
open Client.Menu

open LoginCommon
open LoginPage
open RegisterPage
open ForgotPasswordPage
open PasswordResetPage
open LoginFlowPage
open Shared.Auth
open Shared.WsBridge

importAll "../../node_modules/bootstrap/dist/css/bootstrap.min.css"
importAll "../Client/lib/css/inspinia/style.css"
importAll "../Client/lib/css/inspinia/main.css"
let ethHost = match Utils.load<string> "EthereumHost" with
                | Some eh -> eh
                | None -> 
                    let defaultHost = "http://127.0.0.1:8545"
                    Utils.save "EthereumHost" defaultHost
                    defaultHost

// let web3: Web3Impl = !!createNew web3 (createNew httpProvider ethHost)
// Browser.console.log web3

// open Web3
// open Web3Types
// open Fable.Import.BigNumber

// [<Emit("window.web3")>]
// let web3: Web3 = jsNative
// console.log (sprintf "web3: '%A'" web3)
// console.log (sprintf "web3cp: '%A'" web3.currentProvider)

// // let w3 = web3Factory.Create("http://127.0.0.1:8545" |> U2.Case2 )
// let w3 = web3Factory.Create(web3.currentProvider |> U2.Case1 )

// console.log (sprintf "w3: '%A'" w3)

// // console.log (sprintf "accounts1: '%A'" w3.eth.accounts  ) 
// promise {
//     let! accs = w3.eth.getAccounts()
//     console.log "accs"
//     console.log accs
//     let! bal = w3.eth.getBalance(accs.[0])
//     console.log "bal"
//     console.log (bal / 1000000000000000000.)

//     let! coinbase = w3.eth.getCoinbase()
//     console.log "coinbase"
//     console.log (coinbase)
//     let amount = w3.utils.toWei("1" |> U3.Case1, Web3Types.Unit.Ether)
    
//     // let provider = web3.currentProvider :> obj :?> IProvider
//     // let! _ = provider.send(jsOptions<JsonRPCRequest>(fun r ->  r.method <- "personal_sign" |> Some 
//     //                                                            r.``to`` <- Some "0xC25FdBeaD74a9A1d09F3209d2fcDE652d4D359fE" )) 

//     let tr = jsOptions<Tx>(fun  tx -> tx.value <- amount |> Some 
//                                       tx.from <- Some coinbase
//                                       tx.``to`` <- Some "0xC25FdBeaD74a9A1d09F3209d2fcDE652d4D359fE" )
 
//     //coinbase,"0xC25FdBeaD74a9A1d09F3209d2fcDE652d4D359fE"

//     let! tx = w3.eth.sendTransaction tr

//     console.log tx

//     let! balance = w3.eth.getBalance("0xC25FdBeaD74a9A1d09F3209d2fcDE652d4D359fE")
//     // let! sendSignedTransaction = w3.eth.sendSignedTransaction(coinbase,"0xC25FdBeaD74a9A1d09F3209d2fcDE652d4D359fE" )
//     // let! newAccount = w3.eth.personal.newAccount("123")
//     // console.log "newAccount"
//     // console.log (newAccount)
//     console.log "getBalanse"
//     console.log (balance)
//     let! accs = w3.eth.getAccounts()
//     console.log accs
// }
// |> Promise.start

// 0x22eB149b6885c07B4BB2dC5F374c1DA904e062cE aac
// let www3: obj = importAll "../Client/W3.ts"
// Browser.console.log (sprintf "www3: '%A'" www3)

// let w = w3.W3
// Browser.console.log w3

// Browser.console.log w



// let w3 = !!createNew W3 ()
// Browser.console.log "W3"
// Browser.console.log w3

//let W3 = importDefault<Web3> "soltsice"
// console.log("W3: " + (string W3))
// let web3 = W3.web3
// console.log("web3: " + (string W3.web3))

module LocalStorage = 
    let loadUser () : AuthModel option =
        BrowserLocalStorage.load "user"

    let saveUserCmd (authModel: AuthModel) =
        Cmd.ofFunc (BrowserLocalStorage.save "user") authModel (fun _ -> BrowserStorageUpdated |> UIMsg) (BrowserStorageFailure >> UnexpectedMsg)

    let deleteUserCmd =
        Cmd.ofFunc BrowserLocalStorage.delete "user" (fun _ -> BrowserStorageUpdated |> UIMsg) (BrowserStorageFailure >> UnexpectedMsg)

module Server =

    open Shared
    open Fable.Remoting.Client

    /// A proxy you can use to talk to server directly
    let loginFlowApi : ILoginFlowProtocol =
        Proxy.remoting<ILoginFlowProtocol> {
            use_route_builder Route.builder
        }

    let tokenSaleApi : ITokenSaleProtocol =
        Proxy.remoting<ITokenSaleProtocol> {
            use_route_builder Route.builder
        }        

    let adminApi : IAdminProtocol =
        Proxy.remoting<IAdminProtocol> {
            use_route_builder Route.builder
        }

type BridgeClientMsg = 
    | ClientMsg of WsBridge.ClientMsg
    | AppMsg    of AppMsg

type BridgedMsg = Msg<WsBridge.ServerMsg, BridgeClientMsg>

let msgMapC f = function    | C m -> m |> f |> C 
                            | S m -> S m


let cmdServerCall (apiFunc: 'T -> Async<ServerResult<'R>>) (args: 'T) (completeMsg: 'R -> AppMsg) serverMethodName =
    Cmd.ofAsync
        apiFunc
        args
        (fun res -> match res with
                    | Ok cc             -> cc |> completeMsg |> AppMsg
                    | Error serverError -> serverError |> ServerError |> ServerErrorMsg |> UnexpectedMsg |> AppMsg
                    )
        (fun exn -> console.error(sprintf "Exception during %s call: '%A'" serverMethodName exn)
                    exn |> CommunicationError |> ServerErrorMsg |> UnexpectedMsg |> AppMsg)

let init wsBridgeModel urlParsingResult : AppModel * Cmd<BridgeClientMsg> =
    Browser.console.log (sprintf "Passed Url: '%A'" urlParsingResult)
    let model = {   Loading                 = false
                    Page                    = MenuPage.Default
                    PageModel               = NoPageModel
                    WsBridgeModel           = wsBridgeModel
                }
    let cmd = match LocalStorage.loadUser() with 
                | Some authModel -> authModel.Token |> AuthMsg.LoggedIn |> AuthMsg |> AppMsg |> Cmd.ofMsg
                | None -> Cmd.none  
    model, cmd

let update (msg : AppMsg) (model : AppModel) : AppModel * Cmd<BridgeClientMsg> =
    let enforceLogin model =
        let deleteAuthModelCmd = LocalStorage.deleteUserCmd |> Cmd.map AppMsg
        let loginFlowModel, cmd = LoginFlowPage.init ()
        let cmd = Cmd.batch [   deleteAuthModelCmd
                                DisconnectUserOnServer |> ClientMsg |> Cmd.ofMsg
                                Cmd.map (LoginFlowMsg >> AppMsg) cmd ]
        { model with    Page = MenuPage.LoginFlow LoginFlowPage.Default
                        PageModel = loginFlowModel |> PageModel.LoginFlowModel } , cmd
    let toastrSuccess text =   
                    Toastr.message text
                    |> Toastr.withProgressBar
                    |> Toastr.position BottomRight
                    |> Toastr.timeout 1000
                    |> Toastr.success

    let (model', cmd') : AppModel * Cmd<BridgeClientMsg> =  
        match msg with
        | AuthMsg(AuthMsg.LoggedIn authToken) -> 
            let page = CabinetPagePage.Default
            let cmdLocalStorage             = LocalStorage.saveUserCmd { AuthModel.Token = authToken } |> Cmd.map AppMsg
            let cmdGetCryptoCurrencies      = cmdServerCall (Server.tokenSaleApi.getCryptoCurrencies) () (CabinetModel.GetCryptoCurrenciesCompleted >> CabinetModel.ServerMsg >> CabinetMsg) "getCryptoCurrencies()"
            let cmdGetTokenSale             = cmdServerCall (Server.tokenSaleApi.getTokenSale) () (CabinetModel.GetTokenSaleCompleted >> CabinetModel.ServerMsg >> CabinetMsg) "getTokenSale()"
            let cmdGetFullCustomerCompleted = cmdServerCall (Server.tokenSaleApi.getFullCustomer) (Auth.secureVoidRequest authToken) (CabinetModel.GetFullCustomerCompleted >> CabinetModel.ServerMsg >> CabinetMsg) "getFullCustomer()"
            let cmdTick                     = Cmd.ofMsg (Tick 0UL |> UIMsg |> AppMsg)
            let cmdConnectWsBridge          = Cmd.ofMsg (authToken |> WsBridge.ConnectUserOnServer |> ClientMsg)
            let cmd' = Cmd.batch [  cmdLocalStorage 
                                    cmdGetCryptoCurrencies
                                    cmdGetTokenSale
                                    cmdGetFullCustomerCompleted
                                    cmdTick
                                    cmdConnectWsBridge ]
            Navigation.newUrl (CabinetPagePage.Default |> MenuPage.Cabinet |> toHash) |> List.map (fun f -> f ignore) |> ignore 
            { model with    Page = MenuPage.Cabinet page
                            PageModel = CabinetPage.init authToken |> PageModel.CabinetModel
                 } , cmd'  // TODO: Add UserName
        | AuthMsg(AuthMsg.LoggedOut) -> enforceLogin model

        | UIMsg msg ->
            match msg with 
            | Tick i -> 
                model, cmdServerCall (Server.tokenSaleApi.getPriceTick) i (CabinetModel.PriceTick >> CabinetModel.ServerMsg >> CabinetMsg) "getPriceTick()"
            | BrowserStorageUpdated -> model, Cmd.none            
            | MenuSelected page -> 
                { model with Page = MenuPage.Cabinet page } , toastrSuccess (sprintf "Menu selected: '%A'" page) 
            | Login -> enforceLogin model
            | Logout -> enforceLogin model
                        

        | UnexpectedMsg msg_ ->
            match msg_ with
            | BrowserStorageFailure _ -> 
                model, ("Browser storage access failed with", msg, string model) |> ErrorMsg |> AppMsg |> Cmd.ofMsg
            | ServerErrorMsg _ -> 
                model, ("Server error ", msg, string model) |> ErrorMsg |> AppMsg |> Cmd.ofMsg
        | ErrorMsg(text, msg, m) -> 
            console.error(sprintf "%s Msg '%A' on Model '%A'" text msg m)
            let cmd = Toastr.message text
                    |> Toastr.withProgressBar
                    |> Toastr.position BottomRight
                    |> Toastr.timeout 1000
                    |> Toastr.error
            model, cmd

        | LoginFlowMsg msg_ ->
            match model.PageModel with
            | LoginFlowModel loginModel -> 
                let model', cmd', externalMsg' = LoginFlowPage.update msg_ loginModel
                let cmd2 =
                    match externalMsg' with
                    | LoginFlowPage.ExternalMsg.NoOp -> Cmd.none
                    | LoginFlowPage.ExternalMsg.LoginUser info -> 
                        cmdServerCall (Server.loginFlowApi.login) info (LoginAttemptResult >> LoginPageMsg >> LoginFlowMsg) "login()" 
                    | LoginFlowPage.ExternalMsg.RegisterUser info -> 
                        cmdServerCall (Server.loginFlowApi.register) info (RegisteringAttemptResult >> RegisterPageMsg >> LoginFlowMsg) "register()" 
                    | LoginFlowPage.ExternalMsg.ForgotPasswordUser info -> 
                        cmdServerCall (Server.loginFlowApi.forgotPassword) info (ForgotPasswordAttemptResult >> ForgotPasswordPageMsg >> LoginFlowMsg) "resetPassword()" 
                    | LoginFlowPage.ExternalMsg.ResetPassword info -> 
                        cmdServerCall (Server.loginFlowApi.resetPassword) info (ResetAttemptResult >> PasswordResetPageMsg >> LoginFlowMsg) "resetPassword()" 
                    | LoginFlowPage.ExternalMsg.LoggedIn authToken -> authToken |> AuthMsg.LoggedIn |> AuthMsg |> AppMsg |> Cmd.ofMsg

                { model with PageModel = LoginFlowModel model' }, Cmd.batch [ Cmd.map (LoginFlowMsg >> AppMsg) cmd'; cmd2 ]
            | _ -> model, ErrorMsg("Incorrect Message/Model combination for LoginFlow", LoginFlowMsg msg_, (string model)) |> AppMsg |> Cmd.ofMsg           

        | CabinetMsg msg_ ->             
            match model.PageModel with
            | CabinetModel cabinetModel -> 
                let model', cmd' = CabinetPage.update msg_ cabinetModel
                { model with PageModel = CabinetModel model' }, Cmd.map (CabinetMsg >> AppMsg) cmd'
            | _ -> model, ErrorMsg("Incorrect Message/Model combination for Login", CabinetMsg msg_, (string model)) |> AppMsg |> Cmd.ofMsg           

    model', cmd' 


/// Constructs the view for a page given the model and dispatcher.
[<PassGenerics>]
let cabinetPageView p model (dispatch: BridgeClientMsg -> unit) =
    CabinetPage.view p model (CabinetMsg >> AppMsg >> dispatch)        


let sidebarToggle (e: React.MouseEvent) =
    e.preventDefault()
    document.body.classList.toggle("sidebar-hidden") |> ignore

let sidebarMinimize (e: React.MouseEvent) =
    e.preventDefault()
    document.body.classList.toggle("sidebar-minimized") |> ignore
 
let mobileSidebarToggle (e: React.MouseEvent) =
    e.preventDefault()
    document.body.classList.toggle("sidebar-mobile-show") |> ignore

let asideToggle (e: React.MouseEvent) =
    e.preventDefault()
    document.body.classList.toggle("aside-menu-hidden") |> ignore

type [<Pojo>] LoaderProps = {
    active: bool
    spinner: bool
    text: string
}
let loader: LoaderProps -> React.ReactElement = importDefault("react-loading-overlay")

/// Constructs the view for the application given the model.

let mainView page (model: CabinetModel.Model) (dispatch: BridgeClientMsg -> unit) cabinetPageView = 
    let fullCustomer = model.FullCustomer
    div [ Id "wrapper" ]
        [
            Menu.view page (AppMsg.UIMsg >> AppMsg >> dispatch)
            div [ Id "page-wrapper"
                  Class "gray-bg" ] [
                  TopNavbar.navBar fullCustomer (AppMsg.UIMsg >> AppMsg >> dispatch)
                  div [ Class "wrapper wrapper-content animated fadeInRight"]
                      [ 
                        (cabinetPageView page model dispatch)
                           ]

                  Footer.footer
            ]
        ]

/// Constructs the view for the application given the model.
[<PassGenerics>]
let view (model: AppModel) (dispatch: BridgeClientMsg -> unit) =
    match model.PageModel with 
    | LoginFlowModel loginModel -> 
        LoginFlowPage.view loginModel (AppMsg.LoginFlowMsg >> AppMsg >> dispatch)
    | CabinetModel cm -> 
        mainView (match model.Page with | MenuPage.Cabinet p -> p | _ -> CabinetPagePage.Default) 
            cm dispatch cabinetPageView
    | NoPageModel ->
        Browser.console.error("Unsupported model/Auth state combination")
        Login |> UIMsg |> AppMsg |> dispatch 
        HomePage.view()
         

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
open Elmish.Bridge.HMR
open Shared.WsBridge
open Elmish
#endif

let wsBridgeUrlUpdate result model: AppModel * Cmd<BridgeClientMsg> = 
    let model, cmd = PageRouter.urlUpdate result model
    model, cmd |> Cmd.map AppMsg

module wsBridge =
    let init initialState : WsBridgeModel * Cmd<Msg<WsBridge.ServerMsg, WsBridge.ClientMsg>> =
        Disconnected [], Cmd.none
    let update (appCommands: Cmd<AppMsg> -> unit) (msg: WsBridge.ClientMsg) (model: WsBridgeModel) : WsBridgeModel * Cmd<Msg<WsBridge.ServerMsg, WsBridge.ClientMsg>> = 
        console.log ("p.update: " + msg.ToString()) 
        match msg with 
        | ConnectUserOnServer authToken ->
            let msg' = ConnectUser authToken |> S
            match model with
            | Disconnected pending   -> (msg' :: pending) |> Disconnected, Cmd.none
            | Connected              -> model, msg' |> Cmd.ofMsg  
        | DisconnectUserOnServer        -> 
            let msg' = DisconnectUser |> S
            match model with
            | Disconnected pending   -> (msg' :: pending) |> Disconnected, Cmd.none
            | Connected              -> model, msg' |> Cmd.ofMsg  
        
        | ConnectionLost                -> Disconnected [], Cmd.none 
        | ServerConnected               -> 
            match model with
            | Disconnected pending   -> Disconnected [], pending |> List.rev |> List.map Cmd.ofMsg |> Cmd.batch
            | Connected              -> model, Cmd.none
        | UserConnected _               -> Connected, Cmd.none

        | ServerPriceTick prices     -> 
            prices |> PriceTick |> ServerMsg |> CabinetMsg |> Cmd.ofMsg |> appCommands
            model, Cmd.none         

    let view (model: WsBridgeModel) (dispatch: Msg<WsBridge.ServerMsg, WsBridge.ClientMsg> -> unit) =
        div [] []

let appMsgQueue = // A queue
    let mutable buffer = []
    (fun (msg: Cmd<AppMsg>) -> buffer <- msg :: buffer), (fun () -> 
                                                            let ret = buffer |> List.rev
                                                            buffer <- []
                                                            ret)  

let mapProgram (p: Program<_,WsBridgeModel,WsBridge.ClientMsg,_>): Program<_,AppModel,BridgeClientMsg,_> = 
    {   init = fun args -> 
                        let wsModel, cmdB = p.init args
                        let model, cmdA = init wsModel args
                        model, Cmd.batch [ cmdB |> Cmd.map ClientMsg ; cmdA ]
        update = fun (msg: BridgeClientMsg) (model: AppModel) ->
                    console.log ("update: " + msg.ToString())
                    match msg with
                    | ClientMsg m -> 
                        let model', cmd = p.update m model.WsBridgeModel
                        let allCmds = (snd appMsgQueue)() |> List.map (Cmd.map AppMsg) |> List.append [ cmd |> Cmd.map ClientMsg ] |> Cmd.batch
                        { model with WsBridgeModel = model' }, allCmds
                    | AppMsg m -> 
                        let model', cmd = update m model
                        model', cmd
        subscribe = fun model -> model.WsBridgeModel |> p.subscribe |> Cmd.map ClientMsg 
        view = view
        setState = fun model dispatch ->
                        p.setState model.WsBridgeModel (ClientMsg >> dispatch) 
                        view model dispatch |> ignore 
        onError = p.onError
    } 

//bridge wsBridgeInit wsBridgeUpdate wsBridgeView {
bridge wsBridge.init (wsBridge.update (fst appMsgQueue))  wsBridge.view {
    mapped ClientMsg mapProgram
    mapped Bridge.NavigableMapping (Program.toNavigable (parseHash pageParser) wsBridgeUrlUpdate)
#if DEBUG
    simple Program.withConsoleTrace
    simple Program.withDebugger
#endif
    simple (Program.withReactUnoptimized "ac-app")
#if DEBUG
    mapped Bridge.HMRMsgMapping Program.withHMR
#endif
    at Shared.Route.wsBridgeEndpoint
}
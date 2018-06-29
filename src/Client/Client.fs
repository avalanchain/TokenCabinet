module Client.Main

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Elmish.React
open Elmish.Toastr

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Shared

open Fable
open Fable.Core
open Fable.Import.RemoteDev
open Fable.Import.Browser
open Fable.Import
open JsInterop

open web3Impl
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

importAll "../../node_modules/bootstrap/dist/css/bootstrap.min.css"
importAll "../Client/lib/css/inspinia/style.css"
importAll "../Client/lib/css/inspinia/main.css"
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
        Cmd.ofFunc (BrowserLocalStorage.save "user") user (fun _ -> BrowserStorageUpdated |> UIMsg) (BrowserStorageFailure >> UnexpectedMsg)

    let deleteUserCmd =
        Cmd.ofFunc BrowserLocalStorage.delete "user" (fun _ -> BrowserStorageUpdated |> UIMsg) (BrowserStorageFailure >> UnexpectedMsg)

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

let cmdServerCall apiFunc args (completeMsg: 'T -> AppMsg) serverMethodName =
    Cmd.ofAsync
        apiFunc
        args
        (fun res -> match res with
                    | Ok cc -> cc |> completeMsg
                    | Error serverError -> serverError |> ServerError |> ServerErrorMsg |> UnexpectedMsg
                    )
        (fun exn -> console.error(sprintf "Exception during %s call: '%A'" serverMethodName exn)
                    exn |> CommunicationError |> ServerErrorMsg |> UnexpectedMsg)

let init urlParsingResult : AppModel * Cmd<AppMsg> =
    let model = {   Loading                 = false
                    Page                    = MenuPage.Default
                    PageModel               = NoPageModel
                }


    model, Cmd.none

let update (msg : AppMsg) (model : AppModel) : AppModel * Cmd<AppMsg> =
    let enforceLogin model =
        let loginFlowModel, cmd = LoginFlowPage.init ()
        { model with    Page = MenuPage.LoginFlow LoginFlowPage.Default
                        PageModel = loginFlowModel |> PageModel.LoginFlowModel } , Cmd.map LoginFlowMsg cmd

    let (model', cmd') : AppModel * Cmd<AppMsg> =  
        match msg with
        | AuthMsg(AuthMsg.LoggedIn authToken) -> 
            let page = CabinetPagePage.Default
            let cmdLocalStorage             = LocalStorage.saveUserCmd { AuthModel.Token = authToken }
            let cmdGetCryptoCurrencies      = cmdServerCall (Server.tokenSaleApi.getCryptoCurrencies) () (CabinetPage.GetCryptoCurrenciesCompleted >> CabinetPage.ServerMsg >> CabinetMsg) "getCryptoCurrencies()"
            let cmdGetTokenSale             = cmdServerCall (Server.tokenSaleApi.getTokenSale) () (CabinetPage.GetTokenSaleCompleted >> CabinetPage.ServerMsg >> CabinetMsg) "getTokenSale()"
            let cmdGetFullCustomerCompleted = cmdServerCall (Server.tokenSaleApi.getFullCustomer) (Auth.secureVoidRequest authToken) (CabinetPage.GetFullCustomerCompleted >> CabinetPage.ServerMsg >> CabinetMsg) "getFullCustomer()"
            let cmdTick                     = Cmd.ofMsg (Tick 0UL |> UIMsg)
            let cmd' = Cmd.batch [cmdLocalStorage; cmdGetCryptoCurrencies; cmdGetTokenSale; cmdGetFullCustomerCompleted; cmdTick ]
            Navigation.newUrl (CabinetPagePage.Default |> MenuPage.Cabinet |> toHash) |> List.map (fun f -> f ignore) |> ignore 
            { model with    Page = MenuPage.Cabinet page
                            PageModel = CabinetPage.init authToken |> PageModel.CabinetModel
                 } , cmd'  // TODO: Add UserName
        | AuthMsg(AuthMsg.LoggedOut) -> enforceLogin model

        | UIMsg msg ->
            match msg with 
            | Tick i -> 
                model, cmdServerCall (Server.tokenSaleApi.getPriceTick) i (CabinetPage.PriceTick >> CabinetPage.ServerMsg >> CabinetMsg) "getPriceTick()"
            | BrowserStorageUpdated -> model, Cmd.none            
            | MenuSelected page -> 
                let cmd =   Toastr.message (sprintf "Menu selected: '%A'" page)
                            |> Toastr.withProgressBar
                            |> Toastr.position BottomRight
                            |> Toastr.timeout 1000
                            |> Toastr.success
                { model with Page = MenuPage.Cabinet page } , cmd  
            | Login -> enforceLogin model
            | Logout -> enforceLogin model

        | UnexpectedMsg msg_ ->
            match msg_ with
            | BrowserStorageFailure _ -> 
                model, ("Browser storage access failed with", msg, string model) |> ErrorMsg |> Cmd.ofMsg
            | ServerErrorMsg _ -> 
                model, ("Server error ", msg, string model) |> ErrorMsg |> Cmd.ofMsg
        | ErrorMsg(text, msg, m) -> 
            console.error(sprintf "%s Msg '%A' on Model '%A'" text msg m)
            model, Cmd.none

        | LoginFlowMsg msg_ ->
            match model.PageModel with
            | LoginFlowModel loginModel -> 
                let model', cmd', externalMsg' = LoginFlowPage.update msg_ loginModel
                let cmd2 =
                    match externalMsg' with
                    | LoginFlowPage.ExternalMsg.NoOp -> Cmd.none
                    | LoginFlowPage.ExternalMsg.LoginUser loginInfo -> 
                        cmdServerCall (Server.tokenSaleApi.login) loginInfo (AuthMsg.LoggedIn >> AuthMsg) "login()"
                    | LoginFlowPage.ExternalMsg.RegisterUser info -> Cmd.none // TODO: Implement 
                    | LoginFlowPage.ExternalMsg.ForgotPasswordUser info -> Cmd.none // TODO: Implement

                { model with PageModel = LoginFlowModel model' }, Cmd.batch [ Cmd.map LoginFlowMsg cmd'; cmd2 ]
            | _ -> model, ErrorMsg("Incorrect Message/Model combination for LoginFlow", LoginFlowMsg msg_, (string model)) |> Cmd.ofMsg           

        | CabinetMsg msg_ ->             
            match model.PageModel with
            | CabinetModel cabinetModel -> 
                let model', cmd' = CabinetPage.update msg_ cabinetModel
                { model with PageModel = CabinetModel model' }, Cmd.map CabinetMsg cmd'
            | _ -> model, ErrorMsg("Incorrect Message/Model combination for Login", CabinetMsg msg_, (string model)) |> Cmd.ofMsg           

    model', cmd'


/// Constructs the view for a page given the model and dispatcher.
[<PassGenerics>]
let innerPageView model (dispatch: AppMsg -> unit) =
    match model.Page with
    | MenuPage.Home -> HomePage.view() 

    | MenuPage.LoginFlow page -> 
        match model.PageModel with
        | LoginFlowModel m -> (LoginFlowPage.view m (LoginFlowMsg >> dispatch)) 
        | _ -> 
            Browser.console.error(sprintf "Unexpected PageModel for LoginPage:[%A]" model.PageModel)
            div [ ] [ str "Incorrect login model/page" ]

    | MenuPage.Cabinet p ->
        match model.PageModel with
        | CabinetModel sm -> (CabinetPage.view p sm (CabinetMsg >> dispatch))        
        | _ -> 
            Browser.console.error(sprintf "Unexpected PageModel for CabinetPage:[%A]" model.PageModel)
            div [ ] [ str "Incorrect cabinet model/page" ]


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

let mainView (model: AppModel) (dispatch: AppMsg -> unit) innerPageView = 
            div [ Id "wrapper" ]
                [

                        Menu.view model (AppMsg.UIMsg >> dispatch)
                        div [ Id "page-wrapper"
                              Class "gray-bg" ] [
                              TopNavbar.navBar  model.CabinetModel.FullCustomer (AppMsg.UIMsg >> dispatch)
                              div [ Class "wrapper wrapper-content animated fadeInRight"]
                                  [ 
                                    (innerPageView model dispatch)
                                       ]

                              Footer.footer
                        ]
                    // ]
                ]


[<PassGenerics>]
let pageView (model: AppModel) (dispatch: AppMsg -> unit) innerPageView =
    match model.PageModel with 
    | LoginFlowModel loginModel -> LoginFlowPage.view loginModel (AppMsg.LoginFlowMsg >> dispatch)
    | CabinetModel(_) -> mainView model dispatch innerPageView
    | NoPageModel ->
        Browser.console.error("Unsupported model/Auth state combination")
        Login |> UIMsg |> dispatch 
        div [] []
         
    


/// Constructs the view for the application given the model.
[<PassGenerics>]
let view model dispatch = pageView model dispatch innerPageView


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
|> Program.toNavigable (parseHash pageParser) PageRouter.urlUpdate
// |> Program.withSubscription timer
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "ac-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

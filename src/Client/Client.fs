module Client.Main

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
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
open ClientMsgs
open ClientModels
open System.ComponentModel
open Fable.PowerPack
open Shared.Utils
open Client.Menu

// importAll "../../node_modules/bulma/bulma.sass"
// importAll "../../node_modules/bulma-steps/dist/css/bulma-steps.min.css"
// importAll "../Client/lib/css/dashboard.css"
// importAll "../Client/lib/js/dashboard.js"
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
    let model = {   Auth                    = None
                    Loading                 = false
                    Page                    = MenuPage.Default
                    PageModel               = NoPageModel
                    Counter                 = None
                    CryptoCurrencies        = []
                    CurrenciesCurentPrices  = { Prices = [] }
                    TokenSale               = None
                }
    let cmdInitCounter          = cmdServerCall (Server.adminApi.getInitCounter) () (Init >> OldMsg) "getInitCounter()"
    let cmdGetCryptoCurrencies  = cmdServerCall (Server.tokenSaleApi.getCryptoCurrencies) () (GetCryptoCurrenciesCompleted >> ServerMsg) "getCryptoCurrencies()"
    let cmdGetTokenSale         = cmdServerCall (Server.tokenSaleApi.getTokenSale) () (GetTokenSaleCompleted >> ServerMsg) "getTokenSale()"
    let cmdTick                 = Cmd.ofMsg (Tick 0UL |> UIMsg)

    model, (Cmd.batch [cmdInitCounter; cmdGetCryptoCurrencies; cmdGetTokenSale; cmdTick ])

let update (msg : AppMsg) (model : AppModel) : AppModel * Cmd<AppMsg> =
    let (model', cmd') : AppModel * Cmd<AppMsg> =  
        match msg with
        | OldMsg msg_ -> 
            match model, msg_ with
            | { Counter = None }  , Init x      -> { model with Counter = Some x }      , Cmd.none
            | { Counter = Some x }, Increment   -> { model with Counter = Some (x + 1) }, Cmd.none
            | { Counter = Some x }, Decrement   -> { model with Counter = Some (x - 1) }, Cmd.none
            | model, InitDb -> model, cmdServerCall (Server.adminApi.initDb) () (InitDbCompleted >> OldMsg) "InitDb()"
            | model, InitDbCompleted(_) -> { model with Counter = Some (100) } , Cmd.none
            | _ -> model, ("Unhandled", msg, string model) |> ErrorMsg |> Cmd.ofMsg // Catch all for all messages

        | AuthMsg(LoggedIn authToken) -> 
            let authModel = { LoginPage.AuthModel.Token = authToken; LoginPage.AuthModel.UserName = "" }
            let page = CabinetPage.Page.Default
            let model', cmd' = CabinetPage.init authModel page 
            { model with Auth = Some authModel ; Page = MenuPage.Cabinet page; PageModel = PageModel.CabinetModel model' } , cmd' // TODO: Add UserName
        | AuthMsg(LoggedOut)          -> 
            { model with Auth = None } , Cmd.none

        | ServerMsg msg_ ->
            match msg_ with
            | GetCryptoCurrenciesCompleted cc   -> { model with Counter = Some (cc.Length); CryptoCurrencies = cc } , Cmd.none
            | GetTokenSaleCompleted tc          -> { model with TokenSale = Some (tc) } , Cmd.none
            | PriceTick tick                    -> { model with CurrenciesCurentPrices = tick }, Cmd.none

        | UIMsg msg ->
            match msg with 
            | Tick i -> 
                model, cmdServerCall (Server.tokenSaleApi.getPriceTick) i (PriceTick >> ServerMsg) "getPriceTick()"
            | MenuSelected page -> 
                let cmd =   Toastr.message (sprintf "Menu selected: '%A'" page)
                            |> Toastr.withProgressBar
                            |> Toastr.position BottomRight
                            |> Toastr.timeout 1000
                            |> Toastr.success
                { model with Page = MenuPage.Cabinet page } , cmd  
            | Login -> 
                let loginModel, cmd = LoginPage.init model.Auth
                { model with Page = MenuPage.Login; PageModel = loginModel |> LoginModel } , Cmd.none
            | Logout -> 
                { model with Page = MenuPage.Default; Auth = None; PageModel = NoPageModel } , LoggedOut |> AuthMsg |> Cmd.ofMsg

        | UnexpectedMsg msg_ ->
            match msg_ with
            | BrowserStorageFailure _ -> 
                model, ("Browser storage access failed with", msg, string model) |> ErrorMsg |> Cmd.ofMsg
            | ServerErrorMsg _ -> 
                model, ("Server error ", msg, string model) |> ErrorMsg |> Cmd.ofMsg
        | ErrorMsg(text, msg, m) -> 
            console.error(sprintf "%s Msg '%A' on Model '%A'" text msg m)
            model, Cmd.none

        | LoginMsg msg_ ->
            match model.PageModel with
            | LoginModel loginModel -> 
                let model', cmd', externalMsg' = LoginPage.update msg_ loginModel
                let cmd2 =
                    match externalMsg' with
                    | LoginPage.ExternalMsg.NoOp -> Cmd.none
                    | LoginPage.ExternalMsg.LoginUser loginInfo -> 
                        cmdServerCall (Server.tokenSaleApi.login) loginInfo (LoggedIn >> AuthMsg) "login()"
                        // AuthMsg

                        // LocalStorage.saveUserCmd newUser.
                { model with PageModel = LoginModel model' }, Cmd.batch [ Cmd.map LoginMsg cmd'; cmd2 ]
            | _ -> model, ErrorMsg("Incorrect Message/Model combination for Login", LoginMsg msg_, (string model)) |> Cmd.ofMsg           
        | CabinetMsg msg_ ->             
            match model.PageModel with
            | CabinetModel cabinetModel -> 
                let model', cmd' = CabinetPage.update msg_ cabinetModel
                { model with PageModel = CabinetModel model' }, Cmd.map CabinetMsg cmd'
            | _ -> model, ErrorMsg("Incorrect Message/Model combination for Login", CabinetMsg msg_, (string model)) |> Cmd.ofMsg           

    model', cmd'

// let view (model : Model) (dispatch : Msg -> unit) =
//     div [ Id "wrapper" ]
//         [ 
//         //   NavBrand.navBrand model dispatch
//           LeftMenu.LeftMenu model (UIMsg >> dispatch)
//         //   ChildMenu.childMenu model dispatch
         
//         //   Container.container [ ]
//           div [ Id "page-wrapper" 
//                 Class "gray-bg"]
//               [  TopNavbar.navBar model dispatch
//                  div [ Class "wrapper wrapper-content animated fadeInRight"]
//                     [ 
//                       ContentView.contentView model dispatch
//                        ]]
//           Footer.footer             
//                          ]

/// Constructs the view for a page given the model and dispatcher.
[<PassGenerics>]
let innerPageView model (dispatch: AppMsg -> unit) =
    match model.Page with
    | MenuPage.Home -> [ Home.view() ]

    // | MenuPage.Admin -> 
    //     [ Admin.view (model.Trading) (AdminMsg >> dispatch) ]

    | MenuPage.Login -> 
        match model.PageModel with
        | LoginModel m -> [ LoginPage.view m (LoginMsg >> dispatch) ]
        | _ -> 
            Browser.console.error(sprintf "Unexpected PageModel for LoginPage:[%A]" model.PageModel)
            [ ]

    | MenuPage.Cabinet p ->
        match model.PageModel with
        | CabinetModel sm -> [ CabinetPage.view p sm (CabinetMsg >> dispatch) ]         
        | _ -> 
            Browser.console.error(sprintf "Unexpected PageModel for CabinetPage:[%A]" model.PageModel)
            [ ]

    // | MenuPage.Trading p ->
    //     match model.EthConnection with
    //     | Some ethConnection -> [ (Trading.view p (model.Trading) (ethConnection.EthDispatcher) (LoadingMsg >> dispatch) ) ] 
    //     | None -> [ ]        


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
[<PassGenerics>]
let pageView (model: AppModel) (dispatch: AppMsg -> unit) innerPageView =
    div [ ClassName "app" ]
        [
            // fn loader { active = model.Loading; spinner = true; text = "Talking to Ethereum ..." } [
                header [ ClassName "app-header navbar" ] [
                    button [ClassName "navbar-toggler mobile-sidebar-toggler d-lg-none" 
                            OnClick mobileSidebarToggle 
                            Type "button"] [ unbox "\u9776" ]
                    a [ ClassName "navbar-brand" 
                        Href "#"] []
                    ul [ ClassName "nav navbar-nav d-md-down-none" ] [
                        li [ ClassName "nav-item" ] [
                            button [    ClassName "nav-link navbar-toggler sidebar-toggler" 
                                        Type "button" 
                                        OnClick sidebarToggle ] [ unbox "\u9776" ]
                        ]
                        li [ ClassName "nav-item px-3" ] [
                            a [ ClassName "nav-link" 
                                Href "#" ] [ ofString "Activities" ]
                        ]
                        li [ ClassName "nav-item px-3" ] [
                            a [ ClassName "nav-link" 
                                Href "#" ] [ ofString "Pending requests" ]
                        ]
                    ]
                    ul [ ClassName "nav navbar-nav ml-auto" ] [
                        li [ ClassName "nav-item d-md-down-none" ] [
                            a [ ClassName "nav-link" 
                                Href "#" ] [ 
                                    i [ ClassName "icon-bell" ] []
                                    span [ ClassName "badge badge-pill badge-danger" ] [ ofInt 5 ]
                                ]
                        ]
                        li [ ClassName "nav-item d-md-down-none" ] [
                            a [ ClassName "nav-link" 
                                Href "#" ] [ 
                                    i [ ClassName "icon-list" ] []
                                ]
                        ]
                        li [ ClassName "nav-item d-md-down-none" ] [
                            a [ ClassName "nav-link" 
                                Href "#" ] [ 
                                    i [ ClassName "icon-location-pin" ] []
                                ]
                        ]
                        li [ ClassName "nav-item d-md-down-none" ] [
                            div [ ClassName "dropdown" ] [
                                a [ ClassName "nav-link dropdown-toggle nav-link" ] [
                                    img [   Src "img/avatars/6.jpg" 
                                            ClassName "img-avatar" 
                                            Alt "info@avalanchain.com" ]
                                    ]
                                ]
                        ]
                        // li [ ClassName "nav-item d-md-down-none" ] [
                        //     button [    ClassName "nav-link navbar-toggler aside-menu-toggler" 
                        //                 Type "button" 
                        //                 OnClick asideToggle ] [ unbox "&#9776;" ]
                        // ]
                    ]
                ]

                div [ ClassName "app-body" ] [
                    //sidebar model dispatch
                    Menu.view model (AppMsg.UIMsg >> dispatch)
                    main [ ClassName "main" ] [
                        ol [ ClassName "breadcrumb" ] (
                            match model.Page with 
                            | MenuPage.Home -> [ li [ ClassName "breadcrumb-item active" ] [ ofString "Home" ] ]
                            // | MenuPage.Admin -> [ li [ ClassName "breadcrumb-item active" ] [ ofString "Admin" ] ]
                            | MenuPage.Login -> [ li [ ClassName "breadcrumb-item active" ] [ ofString "Login" ] ]
                            | MenuPage.Cabinet p -> [   li [ ClassName "breadcrumb-item" ] [ ofString "Token Cabinet" ]
                                                        li [ ClassName "breadcrumb-item active" ] [ ofString (getUnionCaseNameSplit p (typeof<CabinetPage.Page>)) ] ]
                            // | MenuPage.Trading p -> [   li [ ClassName "breadcrumb-item" ] [ ofString "Trading" ]
                            //                             li [ ClassName "breadcrumb-item active" ] [ ofString (p |> getUnionCaseNameSplit) ] ]
                        )
                        div [ ClassName "container-fluid" ] [
                            div [ ClassName "animated fadeIn" ] [
                                div [  ] (innerPageView model dispatch)
                            ]
                        ]
                    ]
                    //aside model dispatch
                ]

                footer [ ClassName "app-footer" ] [ 
                    a [ Href "http://www.avalanchain.com" ] [ ofString "2018 Avalanchain" ]
                ]
            // ]
        ]

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
|> Program.toNavigable(parseHash PageRouter.pageParser) PageRouter.urlUpdate
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

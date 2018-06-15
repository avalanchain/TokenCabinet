module Client.App

open FSharp.Reflection

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open ServerCode.Utils
open ServerCode.Domain
open ServerCode.Commodities
open ServerCode.EntityList

open Client
open Client.Messages
open Client.Entity
open Client.Menu
open Client.Model

module RS = Fable.Import.ReactStrap

let staticPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
    Statics.staticEntityDefs 
    |> List.map (fun ed -> map (ed |> MenuPage.Static) (s "static" </> s (getUnionCase(ed).Name.ToLowerInvariant())))


let [<PassGenerics>]tradingPageParsers: Parser<MenuPage -> MenuPage, MenuPage> list = 
    let parserWithUid caseName case getName = 
        map (fun uid -> { Trading.CompanyInfo.UID = uid |> uint32; Trading.CompanyInfo.Name = uid |> uint32 |> getName } |> case |> MenuPage.Trading) (s "trading" </> s caseName </> i32)
    Trading.pageDefs 
    |> List.map (fun ed -> 
                    let pd = ed |> fst
                    match pd with 
                    | Trading.Page.Trader ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Trader (fun uid -> (Examples.traders |> List.find (fun t -> t.Uid = uid)).Name)
                    | Trading.Page.VesselOperator ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.VesselOperator (fun uid -> (Examples.vesselOperators |> List.find (fun t -> t.Uid = uid)).Name)
                    | Trading.Page.VesselMaster ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.VesselMaster (fun uid -> (Examples.captains |> List.find (fun t -> t.Uid = uid)).Name)
                    | Trading.Page.Terminal ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Terminal (fun uid -> (Examples.terminals |> List.find (fun t -> t.Uid = uid)).Name)
                    | Trading.Page.Inspector ci -> parserWithUid (getUnionCase(pd).Name.ToLowerInvariant()) Trading.Page.Inspector (fun uid -> (Examples.inspectors |> List.find (fun t -> t.Uid = uid)).Name)
                        
                    | Trading.Page.Archive 
                    | Trading.Page.All -> map (pd |> MenuPage.Trading) (s "trading" </> s (getUnionCase(pd).Name.ToLowerInvariant()) ))
                    


/// The URL is turned into a Result.
let pageParser : Parser<MenuPage -> _,_> =
    oneOf ([map MenuPage.Home (s "home")
            map MenuPage.Login (s "login") 
            map MenuPage.Admin (s "admin") ] 
            @ staticPageParsers
            @ tradingPageParsers )
    
let urlUpdate (result: MenuPage option) model =
    match result with
    | None ->
        Browser.console.error("Error parsing url:")
        ( model, Navigation.modifyUrl (toHash model.Page) )

    | Some (MenuPage.Login as page) ->
        let m,cmd = Login.init model.User
        { model with Page = page; SubModel = LoginModel m }, Cmd.map LoginMsg cmd

    | Some (MenuPage.Static p as page) ->
        match model.User with
        | Some user ->
            let m,cmd = Statics.init user p
            { model with Page = page; SubModel = m |> StaticModel }, Cmd.map (StaticsMsg) cmd
        | None ->
            model, Cmd.ofMsg (Logout |> MenuMsg)

    | Some (MenuPage.Admin as page) ->
        match model.User with
        | Some user ->
            //let m,cmd = Trading.init user p
            //{ model with Page = page; SubModel = m |> TradingModel }, Cmd.map (TradingMsg) cmd
            { model with Page = page; SubModel = NoSubModel }, Cmd.none
        | None ->
            model, Cmd.ofMsg (Logout |> MenuMsg)

    | Some (MenuPage.Trading p as page) ->
        match model.User with
        | Some user ->
            //let m,cmd = Trading.init user p
            //{ model with Page = page; SubModel = m |> TradingModel }, Cmd.map (TradingMsg) cmd
            { model with Page = page; SubModel = NoSubModel }, Cmd.none
        | None ->
            model, Cmd.ofMsg (Logout |> MenuMsg)

    | Some (MenuPage.Home as page) ->
        { model with Page = page }, []

let init result =
    // match Utils.load<Model> "ContractAddress" with
    // | Some m -> urlUpdate result m
    // | None ->
        let user, menuCmd = Menu.init()
        let trading, tradingCmd = Trading.init()
        let eth, ethCmd = Web3Factory.init()
        let m = 
            { Page = MenuPage.Home
              User = user
              Trading = trading
              SubModel = NoSubModel
              Loading = false
              EthConnection = None }

        let m, cmd = urlUpdate result m
        m, [cmd; menuCmd; tradingCmd] @ ethCmd |> Cmd.batch

let update msg model =
    let model, cmd =
        match msg with
        | AppMsg.OpenLogIn ->
            let m, cmd = Login.init None
            { model with
                Page = MenuPage.Login
                SubModel = LoginModel m }, Cmd.batch [cmd; Navigation.modifyUrl (toHash MenuPage.Login) ]

        | StorageFailure e ->
            printfn "Unable to access local storage: %A" e
            model, []

        | LoginMsg msg ->
            match model.SubModel with
            | LoginModel m -> 
                let m, cmd = Login.update msg m
                let cmd = Cmd.map LoginMsg cmd  
                match m.State with
                | Login.LoginState.LoggedIn token -> 
                    let newUser : UserData = { UserName = m.Login.UserName; Token = token }
                    let cmd =              
                        if model.User = Some newUser then cmd else
                        Cmd.batch [cmd
                                   Cmd.ofFunc (Utils.save "user") newUser (fun _ -> AppMsg.LoggedIn) StorageFailure ]

                    { model with 
                        SubModel = LoginModel m
                        User = Some newUser }, cmd
                | _ -> 
                    { model with 
                        SubModel = LoginModel m
                        User = None }, cmd
            | _ -> model, Cmd.none

        | StaticsMsg msg ->
            match model.SubModel with 
            | StaticModel sm ->
                let m, cmd = Statics.update msg sm
                { model with SubModel = m |> StaticModel }, Cmd.map (StaticsMsg) cmd
            | _ -> model, Cmd.none

        | AdminMsg msg ->
            let m, cmd = Admin.update msg (model.Trading)
            { model with Trading = m }, Cmd.map AdminMsg cmd

        | TradingMsg msg ->
            let m, cmd = Trading.update msg (model.Trading)
            { model with Trading = m }, Cmd.map TradingMsg cmd

        | AppMsg.LoggedIn ->
            let nextPage = MenuPage.Default
            let m,cmd = urlUpdate (Some nextPage) model
            match m.User with
            | Some user ->
                m, Cmd.batch [cmd; Navigation.modifyUrl (toHash nextPage) ]
            | None ->
                m, Cmd.ofMsg (Logout |> MenuMsg)

        | AppMsg.LoggedOut ->
            { model with
                Page = MenuPage.Home
                SubModel = NoSubModel
                User = None } , 
            Navigation.modifyUrl (toHash MenuPage.Home)

        | AppMsg.MenuMsg msg ->
            match msg with 
            | Logout -> model, Cmd.ofFunc Utils.delete "user" (fun _ -> AppMsg.LoggedOut) StorageFailure

        | LoadingMsg b -> { model with Loading = b }, Cmd.none

        | EthereumMsg msg ->
            let m, cmd = Web3Factory.update msg model
            m, cmd
        
    //Utils.save "MODEL" model
    model, cmd

// VIEW

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style


/// Constructs the view for a page given the model and dispatcher.
[<PassGenerics>]
let innerPageView model dispatch =
    match model.Page with
    | MenuPage.Home -> [ Home.view() ]

    | MenuPage.Admin -> 
        [ Admin.view (model.Trading) (AdminMsg >> dispatch) ]

    | MenuPage.Login -> 
        match model.SubModel with
        | LoginModel m -> [ (Login.view m dispatch) ]
        | _ -> [ ]

    | MenuPage.Static p ->
        match model.SubModel with
        | StaticModel sm -> [ (Statics.view p sm dispatch) ]         
        | _ -> 
            Browser.console.error(sprintf "Unexpected SubModel:[%A]" model.SubModel)
            [ ]

    | MenuPage.Trading p ->
        match model.EthConnection with
        | Some ethConnection -> [ (Trading.view p (model.Trading) (ethConnection.EthDispatcher) (LoadingMsg >> dispatch) ) ] 
        | None -> [ ]        


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
let pageView (model: Model) (dispatch: AppMsg -> unit) innerPageView =
    div [ ClassName "app" ]
        [
            fn loader { active = model.Loading; spinner = true; text = "Talking to Ethereum ..." } [
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
                                Href "#" ] [ text "Activities" ]
                        ]
                        li [ ClassName "nav-item px-3" ] [
                            a [ ClassName "nav-link" 
                                Href "#" ] [ text "Pending requests" ]
                        ]
                    ]
                    ul [ ClassName "nav navbar-nav ml-auto" ] [
                        li [ ClassName "nav-item d-md-down-none" ] [
                            a [ ClassName "nav-link" 
                                Href "#" ] [ 
                                    i [ ClassName "icon-bell" ] []
                                    span [ ClassName "badge badge-pill badge-danger" ] [ text 5 ]
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
                    Menu.view model (AppMsg.MenuMsg >> dispatch)
                    main [ ClassName "main" ] [
                        ol [ ClassName "breadcrumb" ] (
                            match model.Page with 
                            | MenuPage.Home -> [ li [ ClassName "breadcrumb-item active" ] [ text "Home" ] ]
                            | MenuPage.Admin -> [ li [ ClassName "breadcrumb-item active" ] [ text "Admin" ] ]
                            | MenuPage.Login -> [ li [ ClassName "breadcrumb-item active" ] [ text "Login" ] ]
                            | MenuPage.Static p -> [li [ ClassName "breadcrumb-item" ] [ text "Static Data" ]
                                                    li [ ClassName "breadcrumb-item active" ] [ text (p |> getUnionCaseNameSplit) ] ]
                            | MenuPage.Trading p -> [   li [ ClassName "breadcrumb-item" ] [ text "Trading" ]
                                                        li [ ClassName "breadcrumb-item active" ] [ text (p |> getUnionCaseNameSplit) ] ]
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
                    a [ Href "http://www.avalanchain.com" ] [ text "2018 Avalanchain" ]
                ]
            ]
        ]

/// Constructs the view for the application given the model.
[<PassGenerics>]
let view model dispatch = pageView model dispatch innerPageView


open Elmish.React

// App
Program.mkProgram init update view
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withConsoleTrace
|> Program.withReact "elmish-app"
// |> Program.withSubscription syncSubscription
|> Program.run
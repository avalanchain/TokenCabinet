module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Shared

open Fulma

open Fulma.FontAwesome
open Fable
open Fable.Core
open Fable.Import.RemoteDev
open Fable.Import.Browser
open JsInterop

// open W3
open Fable.Core.JsInterop

// let W3 = importDefault<W3> "W3"
// console.log("W3: " + (string W3))
// let web3 = W3.web3
// console.log("web3: " + (string W3.web3))

type Model = {
    Counter: Counter option

    CryptoCurrencies: CryptoCurrencies.CryptoCurrency list

    TokenSale: ViewModels.TokenSale option
}

type RemotingError =
    | CommunicationError of exn
    | ServerError of ServerError

type Msg =
    | Increment
    | Decrement
    | Init of Result<Counter, RemotingError>

    | ErrorMsg of Model * Msg

    | InitDb
    | InitDbCompleted of Result<unit, exn>

    | GetCryptoCurrenciesCompleted of Result<CryptoCurrencies.CryptoCurrency list, RemotingError>
    | GetTokenSaleCompleted of Result<ViewModels.TokenSale, RemotingError>

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

let cmdServerCall apiFunc args completeMsg serverMethodName =
    Cmd.ofAsync
        apiFunc
        args
        (fun res -> match res with
                    | Ok cc -> cc |> Ok |> completeMsg
                    | Error serverError -> serverError |> ServerError |> Error |> completeMsg
                    )
        (fun exn -> console.error(sprintf "Exception during %s call: '%A'" serverMethodName exn)
                    exn |> CommunicationError |> Error |> GetCryptoCurrenciesCompleted)

let init () : Model * Cmd<Msg> =
    let model = {   Counter = None
                    CryptoCurrencies = []
                    TokenSale = None 
                }
    let cmdInitCounter          = cmdServerCall (Server.adminApi.getInitCounter) () Init "getInitCounter()"
    let cmdGetCryptoCurrencies  = cmdServerCall (Server.tokenSaleApi.getCryptoCurrencies) () GetCryptoCurrenciesCompleted "getCryptoCurrencies()"
    let cmdGetTokenSale         = cmdServerCall (Server.tokenSaleApi.getTokenSale) () GetTokenSaleCompleted "getTokenSale()"

    model, (Cmd.batch [cmdInitCounter; cmdGetCryptoCurrencies; cmdGetTokenSale])

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    console.log(sprintf "Msg: '%A', Model: '%A'" msg model)
    let (model', cmd') : Model * Cmd<Msg> =  
                        match model, msg with
                        | { Counter = None }  , Init (Ok x) -> { model with Counter = Some x }      , Cmd.none
                        | { Counter = Some x }, Increment   -> { model with Counter = Some (x + 1) }, Cmd.none
                        | { Counter = Some x }, Decrement   -> { model with Counter = Some (x - 1) }, Cmd.none

                        | model, InitDb -> model, Cmd.ofAsync
                                                    Server.adminApi.initDb
                                                    ()
                                                    (Ok >> InitDbCompleted)
                                                    (fun exn -> console.error(sprintf "Exception during InitDb() call: '%A'" exn)
                                                                exn |> Error |> InitDbCompleted)
                        | model, InitDbCompleted(_) -> { model with Counter = Some (100) } , Cmd.none

                        | model, GetCryptoCurrenciesCompleted ccRes -> 
                            match ccRes with 
                            | Ok (cc) -> { model with Counter = Some (cc.Length); CryptoCurrencies = cc } , Cmd.none
                            | Error(error) -> failwith "Not Implemented" // TODO: Implement

                        | model, GetTokenSaleCompleted tcRes -> 
                            match tcRes with 
                            | Ok (tc) -> { model with TokenSale = Some (tc) } , Cmd.none
                            | Error(error) -> failwith "Not Implemented" // TODO: Implement

                        | model, ErrorMsg(m, msg) -> 
                            console.error(sprintf "Unhandled Msg '%A' on Model '%A'" msg m)
                            model, Cmd.none
                        | model, msg -> model, (model, msg) |> ErrorMsg |> Cmd.ofMsg // Catch all for all messages
    model', cmd'

let safeComponents =
    let intersperse sep ls =
        List.foldBack (fun x -> function
            | [] -> [x]
            | xs -> x::sep::xs) ls []

    let components =
        [
            "Saturn", "https://saturnframework.github.io/docs/"
            "Fable", "http://fable.io"
            "Elmish", "https://elmish.github.io/elmish/"
            "Fulma", "https://mangelmaxime.github.io/Fulma"
            "Bulma\u00A0Templates", "https://dansup.github.io/bulma-templates/"
            "Fable.Remoting", "https://zaid-ajaj.github.io/Fable.Remoting/"
        ]
        |> List.map (fun (desc,link) -> a [ Href link ] [ str desc ] )
        |> intersperse (str ", ")
        |> span [ ]

    p [ ]
        [ strong [] [ str "SAFE Template" ]
          str " powered by: "
          components ]

let show = function
            | Some x -> string x
            | None -> "Loading..."

let navBrand =
    Navbar.navbar [ Navbar.Color IsWhite ]
        [ Container.container [ ]
            [ Navbar.Brand.div [ ]
                [ Navbar.Item.a [ Navbar.Item.CustomClass "brand-text" ]
                      [ str "TOKEN SALE" ]
                  Navbar.burger [ ]
                      [ span [ ] [ ]
                        span [ ] [ ]
                        span [ ] [ ] ] ]
              Navbar.menu [ ]
                  [ Navbar.Start.div [ ]
                      [ Navbar.Item.a [ ]
                            [ str "Verification" ]
                        Navbar.Item.a [ ]
                            [ str "Purchase token" ]
                        Navbar.Item.a [ ]
                            [ str "My investments" ]
                        Navbar.Item.a [ ]
                            [ str "Referral Program" ]
                        Navbar.Item.a [ ]
                            [ str "Contacts" ] ] ] ] ]

let menu =
    Menu.menu [ ]
        [ Menu.label [ ]
              [ str "General" ]
          Menu.list [ ]
              [ Menu.item [ ]
                    [ str "Dashboard" ]
                Menu.item [ ]
                    [ str "Customers" ] ]
          Menu.label [ ]
              [ str "Administration" ]
          Menu.list [ ]
              [ Menu.item [ ]
                  [ str "Team Settings" ]
                li [ ]
                    [ a [ ]
                        [ str "Manage Your Team" ]
                      Menu.list [ ]
                          [ Menu.item [ ]
                                [ str "Members" ]
                            Menu.item [ ]
                                [ str "Plugins" ]
                            Menu.item [ ]
                                [ str "Add a member" ] ] ]
                Menu.item [ ]
                    [ str "Invitations" ]
                Menu.item [ ]
                    [ str "Cloud Storage Environment Settings" ]
                Menu.item [ ]
                    [ str "Authentication" ] ]
          Menu.label [ ]
              [ str "Transactions" ]
          Menu.list [ ]
              [ Menu.item [ ]
                    [ str "Payments" ]
                Menu.item [ ]
                    [ str "Transfers" ]
                Menu.item [ ]
                    [ str "Balance" ] ] ]

let breadcrump =
    Breadcrumb.breadcrumb [ ]
        [ Breadcrumb.item [ ]
              [ a [ ] [ str "Bulma" ] ]
          Breadcrumb.item [ ]
              [ a [ ] [ str "Templates" ] ]
          Breadcrumb.item [ ]
              [ a [ ] [ str "Examples" ] ]
          Breadcrumb.item [ Breadcrumb.Item.IsActive true ]
              [ a [ ] [ str "Admin" ] ] ]

let hero =
    Hero.hero [ Hero.Color IsInfo
                Hero.CustomClass "welcome" ]
        [ Hero.body [ ]
            [ Container.container [ ]
                [ Heading.h1 [ ]
                      [ str ("Purchase tokens " (*+ (string W3.version)*) ) ]
                  Heading.h4 [ Heading.IsSubtitle ]
                      [ safeComponents ] ] ] ]

let info (model : Model) (dispatch : Msg -> unit) =
    let tiles =
        match model.TokenSale with
            | Some v -> 
                // [   "Sale Start Date", string v.StartDate
                //     "Sale End Date"  , string v.EndDate
                //     "Soft Cap USD"   , string v.SoftCapUsd
                //     "Hard Cap USD"   , string v.HardCapUsd ] 
                // |> List.map (fun (label, value) -> 
                //                 Tile.parent [ ]
                //                   [ Tile.child [ ]
                //                       [ Box.box' [ ]
                //                           [ Heading.p [ ]
                //                                 [ str value ]
                //                             Heading.p [ Heading.IsSubtitle ]
                //                                 [ str label ] ] ] ] )
                
                let fieldPairs = [  "Sale Start Date", string v.StartDate
                                    "Sale End Date"  , string v.EndDate
                                    "Soft Cap USD"   , string v.SoftCapUsd
                                    "Hard Cap USD"   , string v.HardCapUsd ]

                [ for (label, value) in fieldPairs -> 
                    Tile.parent [ ]
                      [ Tile.child [ ]
                          [ Box.box' [ ]
                              [ Heading.p [ ]
                                    [ str value ]
                                Heading.p [ Heading.IsSubtitle ]
                                    [ str label ] ] ] ] ]

            | None -> [] 

    section [ Class "info-tiles" ]
        [ Tile.ancestor [ Tile.Modifiers [ Modifier.TextAlignment (Fulma.Screen.All, TextAlignment.Centered) ] ]
            tiles                       
        ]

let counter (model : Model) (dispatch : Msg -> unit) =
    Field.div [ Field.IsGrouped ]
        [ Control.p [ ]
            [ Button.a
                [ Button.Color IsInfo
                  Button.OnClick (fun _ -> dispatch InitDb) ]
                [ str "Init Db" ] ]
          Control.p [ Control.IsExpanded ]
            [ Input.text
                [ Input.Disabled true
                  Input.Value (show model.Counter) ] ]
          Control.p [ ]
            [ Button.a
                [ Button.Color IsInfo
                  Button.OnClick (fun _ -> dispatch Increment) ]
                [ str "+" ] ]
          Control.p [ ]
            [ Button.a
                [ Button.Color IsInfo
                  Button.OnClick (fun _ -> dispatch Decrement) ]
                [ str "-" ] ] ]

let columns (model : Model) (dispatch : Msg -> unit) =
    Columns.columns [ ]
        [ Column.column [ Column.Width (Fulma.Screen.All, Column.Is6) ]
            [ Card.card [ CustomClass "events-card" ]
                [ Card.header [ ]
                    [ Card.Header.title [ ]
                        [ str "Events" ]
                      Card.Header.icon [ ]
                          [ Icon.faIcon [ ]
                              [ Fa.icon Fa.I.AngleDown ] ] ]
                  div [ Class "card-table" ]
                      [ Content.content [ ]
                          [ Table.table
                              [ Table.IsFullWidth
                                Table.IsStriped ]
                              [ tbody [ ]
                                  [ for cc in model.CryptoCurrencies ->
                                      tr [ ]
                                          [ td [ Style [ Width "5%" ] ]
                                              [ Icon.faIcon
                                                  [ ]
                                                  [ Fa.icon Fa.I.BellO ] ]
                                            td [ ]
                                                [ str (cc.Id + " -2- " + cc.Name) ]
                                            td [ ]
                                                [ Button.a
                                                    [ Button.Size IsSmall
                                                      Button.Color IsPrimary ]
                                                    [ str "Action" ] ] ] ] ] ] ]
                  Card.footer [ ]
                      [ Card.Footer.item [ ]
                          [ str "View All" ] ] ] ]
          Column.column [ Column.Width (Fulma.Screen.All, Column.Is6) ]
              [ Card.card [ ]
                  [ Card.header [ ]
                      [ Card.Header.title [ ]
                          [ str "Inventory Search" ]
                        Card.Header.icon [ ]
                            [ Icon.faIcon [ ]
                                [ Fa.icon Fa.I.AngleDown ] ] ]
                    Card.content [ ]
                        [ Content.content [ ]
                            [ Control.div
                                [ Control.HasIconLeft
                                  Control.HasIconRight ]
                                [ Input.text
                                      [ Input.Size IsLarge ]
                                  Icon.faIcon
                                      [ Icon.Size IsMedium
                                        Icon.IsLeft ]
                                      [ Fa.icon Fa.I.Search ]
                                  Icon.faIcon
                                      [ Icon.Size IsMedium
                                        Icon.IsRight ]
                                      [ Fa.icon Fa.I.Check ] ] ] ] ]
                Card.card [ ]
                    [ Card.header [ ]
                        [ Card.Header.title [ ]
                              [ str "Counter" ]
                          Card.Header.icon [ ]
                              [ Icon.faIcon [ ]
                                  [ Fa.icon Fa.I.AngleDown ] ] ]
                      Card.content [ ]
                        [ Content.content   [ ]
                            [ counter model dispatch ] ] ]   ] ]

let view (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [ navBrand
          Container.container [ ]
              [ Columns.columns [ ]
                  [ Column.column [ Column.Width (Fulma.Screen.All, Column.Is3) ]
                      [ menu ]
                    Column.column [ Column.Width (Fulma.Screen.All, Column.Is9) ]
                      [ //breadcrump
                        hero
                        info model dispatch
                        columns model dispatch ] ] ] ]


#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

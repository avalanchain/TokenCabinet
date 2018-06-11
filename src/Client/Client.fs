module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Shared

open Fulma

open Fulma.FontAwesome
open Fable.Core
open Fable.Import.RemoteDev

// open W3
open Fable.Core.JsInterop

//let w3:W3 = !!createNew W3 () 

// [<Emit("$0")>]
// let web3: W3 = jsNative

type Model = {
    Counter: Counter option

    CryptoCurrencies: CryptoCurrencies.CryptoCurrency list
}

type RemotingError =
    | CommunicationError of exn
    | ServerError of ServerError

type Msg =
    | Increment
    | Decrement
    | Init of Result<Counter, exn>

    | ErrorMsg of Model * Msg

    | InitDb
    | InitDbCompleted of Result<unit, exn>

    | GetCryptoCurrenciesCompleted of Result<CryptoCurrencies.CryptoCurrency list, RemotingError>

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


let init () : Model * Cmd<Msg> =
    let model = {   Counter = None
                    CryptoCurrencies = [] 
                }
    let cmdInitCounter =
        Cmd.ofAsync
            Server.adminApi.getInitCounter
            ()
            (Ok >> Init)
            (fun exn -> printfn "Exception during InitCounter() call: '%A'" exn
                        exn |> Error |> Init)
    let cmdGetCryptoCurrencies =
        Cmd.ofAsync
            Server.tokenSaleApi.getCryptoCurrencies
            ()
            (fun res -> match res with
                        | Ok cc -> cc |> Ok |> GetCryptoCurrenciesCompleted
                        | Error serverError -> serverError |> ServerError |> Error |> GetCryptoCurrenciesCompleted
                        )
            (fun exn -> printfn "Exception during GetCryptoCurrencies() call: '%A'" exn
                        exn |> CommunicationError |> Error |> GetCryptoCurrenciesCompleted)
    model, (Cmd.batch [cmdInitCounter; cmdGetCryptoCurrencies])

let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    let (model', cmd') : Model * Cmd<Msg> =  
                        match model, msg with
                        | { Counter = None }  , Init (Ok x) -> { model with Counter = Some x }      , Cmd.none
                        | { Counter = Some x }, Increment   -> { model with Counter = Some (x + 1) }, Cmd.none
                        | { Counter = Some x }, Decrement   -> { model with Counter = Some (x - 1) }, Cmd.none

                        | model, InitDb -> model, Cmd.ofAsync
                                                    Server.adminApi.initDb
                                                    ()
                                                    (Ok >> InitDbCompleted)
                                                    (fun exn -> printfn "Exception during InitDb() call: '%A'" exn
                                                                exn |> Error |> InitDbCompleted)
                        | model, InitDbCompleted(_) -> { model with Counter = Some (100) } , Cmd.none

                        | model, GetCryptoCurrenciesCompleted ccRes -> 
                            match ccRes with 
                            | Ok (cc) -> { model with Counter = Some (cc.Length); CryptoCurrencies = cc } , Cmd.none
                            | Error(error) -> failwith "Not Implemented" // TODO: Implement

                        | model, ErrorMsg(m, msg) -> 
                            printfn "Unhandled Msg '%A' on Model '%A'" msg m
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
                      [ str ("Purchase tokens " (*+ (string web3.version) *) ) ]
                  Heading.h4 [ Heading.IsSubtitle ]
                      [ safeComponents ] ] ] ]

let info =
    section [ Class "info-tiles" ]
        [ Tile.ancestor [ Tile.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
            [ Tile.parent [ ]
                  [ Tile.child [ ]
                      [ Box.box' [ ]
                          [ Heading.p [ ]
                                [ str "439k" ]
                            Heading.p [ Heading.IsSubtitle ]
                                [ str "Users" ] ] ] ]
              Tile.parent [ ]
                  [ Tile.child [ ]
                      [ Box.box' [ ]
                          [ Heading.p [ ]
                                [ str "59k" ]
                            Heading.p [ Heading.IsSubtitle ]
                                [ str "Products" ] ] ] ]
              Tile.parent [ ]
                  [ Tile.child [ ]
                      [ Box.box' [ ]
                          [ Heading.p [ ]
                                [ str "3.4k" ]
                            Heading.p [ Heading.IsSubtitle ]
                                [ str "Open Orders" ] ] ] ]
              Tile.parent [ ]
                  [ Tile.child [ ]
                      [ Box.box' [ ]
                          [ Heading.p [ ]
                                [ str "19" ]
                            Heading.p [ Heading.IsSubtitle ]
                                [ str "Exceptions" ] ] ] ] ] ]

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
        [ Column.column [ Column.Width (Screen.All, Column.Is6) ]
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
          Column.column [ Column.Width (Screen.All, Column.Is6) ]
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
                  [ Column.column [ Column.Width (Screen.All, Column.Is3) ]
                      [ menu ]
                    Column.column [ Column.Width (Screen.All, Column.Is9) ]
                      [ breadcrump
                        hero
                        info
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

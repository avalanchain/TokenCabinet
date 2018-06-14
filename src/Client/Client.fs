module Client

open Elmish
open Elmish.React

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

let ethHost = match Utils.load<string> "EthereumHost" with
                | Some eh -> eh
                | None -> 
                    let defaultHost = "http://127.0.0.1:8545"
                    Utils.save "EthereumHost" defaultHost
                    defaultHost

let web3: Web3Impl = !!createNew web3 (createNew httpProvider ethHost)
Browser.console.log web3

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
                    MenuMediator = Contacts 
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



let show = function
            | Some x -> string x
            | None -> "Loading..."

let info (model : Model) (dispatch : Msg -> unit) =
    let tiles =
        let fieldPairs = 
            match model.TokenSale with
                | Some v -> [   "Sale Start Date", v.StartDate.ToShortDateString()
                                "Sale End Date"  , v.EndDate.ToShortDateString()
                                "Soft Cap USD"   , v.SoftCapUsd.ToString()
                                "Hard Cap USD"   , v.HardCapUsd.ToString() ]
                | None -> [ "", "" ]

        [ for (label, value) in fieldPairs -> 
            Tile.parent [ ]
              [ Tile.child [ ]
                  [ Box.box' [ ]
                      [ Heading.p [ ]
                            [ str label ]
                        Heading.p [ Heading.IsSubtitle ]
                            [ str value ] ] ] ] ]                                    

    section [ Class "info-tiles" ]
        [ Tile.ancestor [ Tile.Modifiers [ Modifier.TextAlignment (Fulma.Screen.All, TextAlignment.Centered) ] ]
            tiles                       
        ]

let radio dispatch =
    Field.div [ Field.IsGrouped ]
        [ 
          Control.div [ ]
            [ Button.button
                [   Button.IsLink 
                    Button.OnClick (fun _ -> dispatch InitDb) ]
                [ str "Init Db" ]]
          Control.div [ ]
            [ 
                Radio.radio [ ]
                    [ Radio.input [ Radio.Input.Name "answer" ]
                      str "Yes" ]
                Radio.radio [ ]
                    [ Radio.input [ Radio.Input.Name "answer" ]
                      str "No" ] 
            ]

        //   Control.p [ ]
        //     [ Button.a
        //         [ Button.Color IsInfo
        //           Button.OnClick (fun _ -> dispatch InitDb) ]
        //         [ str "Init Db" ] ]
                
        ]
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
                            [ counter model dispatch ] ] ]   
                Card.card [ ]
                    [ Card.header [ ]
                        [ Card.Header.title [ ]
                              [ str "Radio" ]
                          Card.Header.icon [ ]
                              [ Icon.faIcon [ ]
                                  [ Fa.icon Fa.I.AngleDown ] ] ]
                      Card.content [ ]
                        [ Content.content   [ ]
                            [ radio dispatch ] ] ] ] ]

let view (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [ NavBrand.navBrand model dispatch
          Container.container [ ]
              [ Columns.columns [ ]
                  [ Column.column [ Column.Width (Fulma.Screen.All, Column.Is3) ]
                      [ NavMenu.menu ]
                    Column.column [ Column.Width (Fulma.Screen.All, Column.Is9) ]
                      [ //breadcrump
                        HeroTile.hero
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

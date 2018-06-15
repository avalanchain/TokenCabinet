module Client.PurchaseToken

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open Fable
open Shared.ViewModels
open Fable.DateFunctions
open Fulma.FontAwesome
open System

 
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
                      ] ] ]


let tokenSaleStages  (model : Model) (dispatch : Msg -> unit) =

    let convertDateTime (dt : DateTime) = dt.ToShortDateString()
    let getStatus = function    
                        | Expectation -> ""
                        | Active -> "is-active"
                        | Completed -> " is-completed is-success"
                        | Cancelled -> ""
                        | Paused -> ""
    let getIcon = function    
                            | Expectation -> ""
                            | Active -> ""
                            | Completed -> "fa-check"
                            | Cancelled -> ""
                            | Paused -> ""



    model.TokenSale.Value.TokenSaleStages 
    |> List.mapi (fun idx a -> div [ Class ("step-item  " + (getStatus a.Status)) ]
                                [ div [ Class "step-marker" ]
                                    [ span [ Class "icon" ]
                                        [ i [ Class ("fa " + getIcon a.Status) ]
                                            [ ] ] ]
                                  div [ Class "step-details" ]
                                    [ p [ Class "step-title" ]
                                        [ str (string a.Name)  ]
                                      p [ ]
                                        [ str ( string a.CapEth + " ETH") ] 
                                      p [ Class "is-size-7" ]
                                        [ str ( convertDateTime a.StartDate.Date + " - " + convertDateTime a.EndDate) ]] ] )
    |> div [ Class "steps is-medium"] 
    


let purchaseTokenView  (model : Model) (dispatch : Msg -> unit) = 
    div [ ]
        [ HeroTile.hero
          tokenSaleStages model dispatch
          info model dispatch
          columns model dispatch
          ]


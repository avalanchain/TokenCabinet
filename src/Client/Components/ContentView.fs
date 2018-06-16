module Client.ContentView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open ClientModelMsg
open Fable
open Verifications

let verificationView  = Verification.verificationView


let purchaseTokenView = PurchaseToken.purchaseTokenView        

let show = function
            | Some x -> string x
            | None -> "Loading..."

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

let myInvestmentsView  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [   columns           model dispatch 
        ]
let referralProgramView  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [   str "referralProgramView" 
            br []
            str (string model.CurrenciesCurentPrices)
        ]

let contactsView  (model : Model) (dispatch : Msg -> unit) =
    div [ ]
        [ str "Contacts" ]
        
let contentView  (model : Model) (dispatch : Msg -> unit) =
    match model.MenuMediator with 
    | Verification -> verificationView model dispatch
    | PurchaseToken -> purchaseTokenView model dispatch
    | MyInvestments -> myInvestmentsView model dispatch
    | ReferralProgram -> referralProgramView model dispatch
    | Contacts -> contactsView model dispatch
module Client.LeftMenu

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open Fable.Recharts

let LeftMenu  (model : Model) (dispatch : Msg -> unit)=
    let navItem icon menuMediator =  li [ Class ("side-icon " + if model.MenuMediator = menuMediator then "is-active" else "")
                                          OnClick (fun _ ->  menuMediator |> MenuSelected |> dispatch)  
                                           ]
                                        [ i [ Class icon ]
                                            [ ] ]

    nav [ Class "main-menu" ]
        [ div [ Class "main-menu-inner" ]
            [ ul [ ]
                [ li [ Class "main-logo" ]
                    [ a [ Href "#" ]
                        [ img [ Src "../lib/images/logos/square-violet.svg"
                                Alt "" ] ] ]
                  
                  navItem "sl sl-icon-speedometer" Verification
                  navItem "sl sl-icon-docs" PurchaseToken
                  navItem "sl sl-icon-briefcase" MyInvestments
                  navItem "sl sl-icon-graph" ReferralProgram
                  navItem "sl sl-icon-settings" Contacts
                  navItem "sl sl-icon-settings" Dashboard
                       
                 ] ] ]
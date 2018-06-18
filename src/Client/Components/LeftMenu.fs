module Client.LeftMenu

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open Fable.Recharts

let LeftMenu  (model : Model) (dispatch : Msg -> unit)=
    let navItem name menuMediator = Navbar.Item.a [ Navbar.Item.IsActive (model.MenuMediator = menuMediator)
                                                    Navbar.Item.Props [ OnClick (fun _ ->  menuMediator |> MenuSelected |> dispatch)] ]
                                        [ str name ]
    let navItem icon menuMediator =  li [ Class "side-icon"
                                        //   Props.OnClick [ OnClick (fun _ ->  menuMediator |> MenuSelected |> dispatch)]  
                                          HTMLAttr.Custom ("data-child-menu", "dashboard-menu") ]
                                        [ i [ Class icon ]
                                            [ ] ]

    nav [ Class "main-menu" ]
        [ div [ Class "main-menu-inner" ]
            [ ul [ ]
                [ li [ Class "main-logo" ]
                    [ a [ Href "#" ]
                        [ img [ Src "../lib/images/logos/square-violet.svg"
                                Alt "" ] ] ]
                  li [ Class "side-icon is-active"
                       HTMLAttr.Custom ("data-child-menu", "dashboard-menu") ]
                    [ i [ Class "sl sl-icon-speedometer" ]
                        [ ] ]
                  li [ Class "side-icon"
                       HTMLAttr.Custom ("data-child-menu", "documents-menu") ]
                    [ i [ Class "sl sl-icon-docs" ]
                        [ ] ]
                  li [ Class "side-icon"
                       HTMLAttr.Custom ("data-child-menu", "business-menu") ]
                    [ i [ Class "sl sl-icon-briefcase" ]
                        [ ] ]
                  li [ Class "side-icon"
                       HTMLAttr.Custom ("data-child-menu", "misc-menu") ]
                    [ i [ Class "sl sl-icon-graph" ]
                        [ ] ]
                  li [ Class "side-icon"
                       HTMLAttr.Custom ("data-child-menu", "settings-menu") ]
                    [ i [ Class "sl sl-icon-settings" ]
                        [ ] ] ] ] ]
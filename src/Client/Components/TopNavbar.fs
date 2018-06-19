module Client.TopNavbar

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open JsInterop

importAll "../lib/css/icons.min.css"
importAll "../lib/css/nav.css"

let navBar  (model : Model) (dispatch : Msg -> unit)=
    // let navItem name menuMediator = Navbar.Item.a [ Navbar.Item.IsActive (model.MenuMediator = menuMediator)
    //                                                 Navbar.Item.Props [ OnClick (fun _ ->  menuMediator |> MenuSelected |> dispatch)] ]
    //                                     [ str name ]

    nav [ Class "nav dashboard-nav has-shadow" ]
        [ div [ Class "container is-fluid" ]
        [ div [ Class "nav-left" ]
            [ 
            //   div [ Class "nav-item nav-icon logout-button" ]
            //     [ i [ Class "sl sl-icon-power" ]
            //         [ ] ]
            //   div [ Class "nav-item reader-switch is-hidden-desktop is-hidden-tablet" ]
            //     [ div [ Class "field" ]
            //         [ input [ Id "reader-mode-switch"
            //                   Type "checkbox"
            //                   Name "reader-mode-switch"
            //                   Class "switch is-outlined is-primary is-small" ]
            //           label [ Id "reader-mode-toggle"
            //                   HTMLAttr.Custom ("for", "reader-mode-switch") ]
            //             [ ] ] ]
              div [ Class "nav-item" ]
                [ div [ Class "field is-hidden-mobile" ]
                    [ div [ Class "searchbox control has-icons-left" ]
                        [ input [ Class "input is-secondary-focus"
                                  Type "text"
                                  Placeholder "Quick search" ]
                          span [ Class "icon is-left" ]
                            [ i [ Class "sl sl-icon-magnifier" ]
                                [ ] ] ] ] ]
            //   div [ Class "nav-item nav-icon search-icon modal-trigger is-hidden-desktop is-hidden-tablet"
            //         HTMLAttr.Custom ("data-modal", "search-modal") ]
            //     [ i [ Class "sl sl-icon-magnifier" ]
            //         [ ] ]
            //   div [ Class "nav-item nav-icon chat-button is-hidden-desktop is-hidden-tablet"
            //         HTMLAttr.Custom ("data-show", "quickview")
            //         HTMLAttr.Custom ("data-target", "main-quickview") ]
            //     [ i [ Class "im im-icon-Speach-Bubble11" ]
            //         [ ] ] 
                    ]
          div [ Class "nav-right nav-menu" ]
            [ ] ] ]
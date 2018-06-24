module Client.TopNavbar

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
open JsInterop

// importAll "../lib/css/icons.min.css"
// importAll "../lib/css/nav.css"

let navBar  (model : Model) (dispatch : Msg -> unit)=
    // let navItem name menuMediator = Navbar.Item.a [ Navbar.Item.IsActive (model.MenuMediator = menuMediator)
    //                                                 Navbar.Item.Props [ OnClick (fun _ ->  menuMediator |> MenuSelected |> dispatch)] ]
    //                                     [ str name ]

       div [ Class "row border-bottom" ]
            [ nav [ Class "navbar navbar-static-top white-bg"
                    Role "navigation"
                    // HTMLAttr.Custom ("style", "margin-bottom: 0") 
                    ]
                [ div [ Class "navbar-header logo-bg" ]
                    [ a [ Class "navbar-minimalize minimalize-styl-2 btn btn-primary "
                          Href "#" ]
                        [ i [ Class "fa fa-bars" ]
                            [ ] ]
                      form [ Role "search"
                             Class "navbar-form-custom"
                             Method "post"
                             Action "#" ]
                        [ div [ Class "form-group" ]
                            [ input [ Type "text"
                                      Placeholder "Search for something..."
                                      Class "form-control"
                                      Name "top-search"
                                      Id "top-search" ] ] ] ]
                  ul [ Class "nav navbar-top-links navbar-right" ]
                    [ li [ ]
                        [ a [ Href "#" ]
                            [ i [ Class "fa fa-sign-out" ]
                                [ ]
                              str "Log out" ] ] ] ] ]
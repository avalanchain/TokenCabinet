module Client.TopNavbar

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props



open Client
open ClientMsgs
open ClientModels
open JsInterop


let navBar (dispatch: UIMsg -> unit)  = 
    // let navItem name menuMediator = Navbar.Item.a [ Navbar.Item.IsActive (model.MenuMediator = menuMediator)
    //                                                 Navbar.Item.Props [ OnClick (fun _ ->  menuMediator |> MenuSelected |> dispatch)] ]
    //                                     [ str name ]

       div [ Class "row border-bottom" ]
            [ nav [ Class "navbar navbar-static-top white-bg no-margins"
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
                        [ a [ Href "#" 
                              OnClick(fun _ -> dispatch Logout)  ]
                            [ i [ Class "fa fa-sign-out" ]
                                [ ]
                              str "Log out" ] ] ] ] ]
module Client.TopNavbar

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props



open Client
open ClientMsgs
open ClientModels
open JsInterop
open Shared.Auth
open CabinetModel
open Shared.ViewModels


let customer (fullCustomer: FullCustomer option) =
    match fullCustomer with
            | Some f -> f.Customer.Email
            | None   -> "" 

let navBar (fullCustomer: FullCustomer option) (dispatch: UIMsg -> unit)  = 
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
                            [ i [ Class "fa fa-envelope" ]
                                [ ]
                              str (customer fullCustomer) ] ]

                      li [ ]
                         [ a [ Href "#" 
                               OnClick(fun _ -> dispatch Logout)  ]
                            [ i [ Class "fa fa-sign-out" ]
                                [ ]
                              str "Log out" ] ] ] ] ]
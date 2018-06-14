module Client.Breadcrumb

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

let bc =
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
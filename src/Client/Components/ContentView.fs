module Client.ContentView

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg
 
let navBrand  (model : Model) (dispatch : Msg -> unit)=
    let navItem name menuMediator = Navbar.Item.a [ Navbar.Item.IsActive (model.MenuMediator = menuMediator)
                                                    Navbar.Item.Props [ OnClick (fun _ ->  menuMediator |> MenuSelected |> dispatch)] ]
                                        [ str name ]

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
                      [ navItem "Verification" Verification
                        navItem "Purchase token" PurchaseToken
                        navItem "My investments" MyInvestments
                        navItem "Referral Program" ReferralProgram
                        navItem "Contacts" Contacts
                      ] ] ] ]
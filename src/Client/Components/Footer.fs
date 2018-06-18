module Client.Footer

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientModelMsg

let footer  =
    footer [ Class "footer" ]
           [ div [ Class "container" ]
                [ div [ Class "has-text-centered" ]
                    [ div [ Class "has-text-centered" ]
                        [ img [ Class "small-footer-logo"
                                Src "assets/images/logos/bulkit-logo-g.png"
                                Alt "" ] ] ]
                  div [ Class "has-text-centered" ]
                    [ span [ Class "more-info-company" ]
                        [ str "Powered by Aavalanchain" ] ] ] ]
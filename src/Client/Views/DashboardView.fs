module Client.DashboardView

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientMsgs
open JsInterop

importAll "../lib/css/icons.min.css"
// importAll "../lib/css/dashboard.css"

// let view  =
//   div []
//       []
let view  =

            div [ Class "row" ]
                [ div [ Class "col-lg-12" ]
                    [ div [ Class "text-center m-t-lg" ]
                        [ h1 [ ]
                            [ str "Simple example of second view" ]
                          small [ ]
                            [ str "Writen in minor.html file." ] ] ] ]
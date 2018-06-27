module Client.DashboardView

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
// open ClientMsgs
open JsInterop


let periods = ["100", "25"; "50", "20"; "20", "15"; "10", "10"]
let view  =
        div [ Class "row" ]
            [ div [ Class "col-lg-12" ]
                [ div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str "Timeline" ] ]
                      div [ Class "ibox-content" ]
                        [ ul [ Class "timeline"
                               Id "timeline" ]
                               [
                                 for (count, discount) in periods ->
                                   li [ Class "li complete" ]
                                      [ div [ Class "timestamp" ]
                                          [ span [ Class "author" ]
                                              [ str (count + " ETH") ] ]
                                        div [ Class "status" ]
                                          [ h4 [ ]
                                              [ str (discount + " %") ] ] ]
                               ]
                             ] ] ] ]





                            //  [ li [ Class "li complete" ]
                            //     [ div [ Class "timestamp" ]
                            //         [ span [ Class "author" ]
                            //             [ str "10 ETH" ] ]
                            //       div [ Class "status" ]
                            //         [ h4 [ ]
                            //             [ str "25 %" ] ] ] ]
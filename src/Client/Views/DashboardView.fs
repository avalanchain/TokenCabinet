module Client.DashboardView

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
// open ClientMsgs
open JsInterop


let periods = ["100", "25", true; "50", "20", true; "20", "15", false; "10", "10", false]
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
                                 for (count, discount, complete) in periods ->
                                   li [ Class ("li" + (if complete then " complete" else "")) ]
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
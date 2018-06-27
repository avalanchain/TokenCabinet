module Client.Ibox

open Fable.Helpers.React
open Fable.Helpers.React.Props


let view title body =
    div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str title ] ]
                      div [ Class "ibox-content" ]
                        body ] 
let viewRow title body =
    div [ Class "row" ]
            [ div [ Class "col-lg-12" ]
                [ view title body ] ]

// let viewCol title body col=
//     div [ Class "ibox float-e-margins" ]
//                     [ div [ Class "ibox-title" ]
//                         [ h5 [ ]
//                             [ str title ] ]
//                       div [ Class "ibox-content" ]
//                         [ body] ]
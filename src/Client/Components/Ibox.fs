module Client.Ibox

open Fable.Helpers.React
open Fable.Helpers.React.Props


let inner title body =
    div [ Class "ibox float-e-margins" ]
                    [ div [ Class "ibox-title" ]
                        [ h5 [ ]
                            [ str title ] ]
                      div [ Class "ibox-content" ]
                        body ] 
let btRow title body =
    div [ Class "row" ]
            [ div [ Class "col-md-12" ]
                [ inner title body ] ]

let btCol title col body =
    div [ Class ("col-md-"+ col) ]
                [ inner title body ]

let emptyRow body =
    div [ Class "row" ]
            body
            
// let btRow7 title body =
//     div [ Class "row seven-cols" ]
//             [ inner title body ]

// let viewCol title body col=
//     div [ Class "ibox float-e-margins" ]
//                     [ div [ Class "ibox-title" ]
//                         [ h5 [ ]
//                             [ str title ] ]
//                       div [ Class "ibox-content" ]
//                         [ body] ]
module Client.Helpers

open Fable.Helpers.React
open Fable.Helpers.React.Props
// open Fulma
// open ClientMsgs
open Fable
open Shared.ViewModels
open Fable.DateFunctions
// open Fulma.FontAwesome
open System
open Fable.Import.React
open Fable.Core.JsInterop

// let toTiles fieldPairs =
//     [ for (label, value) in fieldPairs -> 
//             Tile.parent [ ]
//               [ Tile.child [ ]
//                   [ Box.box' [ ]
//                       [ Heading.p [ ]
//                             [ str label ]
//                         Heading.p [ Heading.IsSubtitle ]
//                             [ str value ] ] ] ] 
//     ]
//     |> Tile.ancestor [ Tile.Modifiers [ Modifier.TextAlignment (Fulma.Screen.All, TextAlignment.Centered) ] ]
//     |> fun ta -> section [ Class "info-tiles" ] [ ta ]

module React =
    type Component<'P> = Fable.Import.React.Component<'P, obj>

let inline com<'P> (com: React.Component<'P>) (props: 'P ) (children: ReactElement seq): ReactElement =
    createElement(com, props, children)
let inline comF<'P> (com: React.Component<'P>) (propsFunc: 'P -> unit) (children: ReactElement seq): ReactElement =
    createElement(com, jsOptions<'P> propsFunc, children)
let inline comE<'P> (com: React.Component<'P>) (children: ReactElement seq): ReactElement =
    createElement(com, createEmpty<'P>, children)

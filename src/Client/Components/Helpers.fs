module Client.Helpers

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open ClientMsgs
open Fable
open Shared.ViewModels
open Fable.DateFunctions
open Fulma.FontAwesome
open System

let toTiles fieldPairs =
    [ for (label, value) in fieldPairs -> 
            Tile.parent [ ]
              [ Tile.child [ ]
                  [ Box.box' [ ]
                      [ Heading.p [ ]
                            [ str label ]
                        Heading.p [ Heading.IsSubtitle ]
                            [ str value ] ] ] ] 
    ]
    |> Tile.ancestor [ Tile.Modifiers [ Modifier.TextAlignment (Fulma.Screen.All, TextAlignment.Centered) ] ]
    |> fun ta -> section [ Class "info-tiles" ] [ ta ]

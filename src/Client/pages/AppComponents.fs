module Client.AppComponents

open FSharp.Reflection

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open Fable.PowerPack
open Fable.Helpers
open Fable.Helpers.React
open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style
open Client.Messages

module RS = Fable.Import.ReactStrap

[<PassGenerics>]
let sidebar model dispatch =
    div [ ClassName "sidebar" ] [
        nav [ ClassName "sidebar-nav" ] [
            // RS.Nav (ignore) [ 
            //     RS.NavItem (ignore) [
            //         RS.NavLink (fun props -> props.href <- Some "#") [ text "TEXT 1" ]
            //         RS.NavLink (fun props -> props.href <- Some "#") [ text "TEXT 2" ]
            //         RS.NavLink (fun props -> props.href <- Some "#") [ text "TEXT 3" ]
            //         RS.NavLink (fun props -> props.href <- Some "#") [ text "TEXT 4" ]
            //     ]
            // ]


            ul [ ClassName "nav" ] [
                li [ ClassName "nav-title" ] [ text " SELLER" ]
                li [ ClassName "nav-item nav-dropdown open" ] [
                    ul [ ClassName "nav-dropdown-items" ] [
                        a [ ClassName "nav-link"; Href "#" ] [
                            i [ ClassName "icon-puzzle" ] [ ]; text " Available actions"
                        ]
                    ]
                ]

                li [ ClassName "nav-title" ] [ text " BUYER" ]
                li [ ClassName "nav-item nav-dropdown open" ] [
                    ul [ ClassName "nav-dropdown-items" ] [
                        a [ ClassName "nav-link"; Href "#" ] [
                            i [ ClassName "icon-puzzle" ] [ ]; text " Available actions"
                        ]
                    ]
                ]

                li [ ClassName "nav-title" ] [ text " VESSEL OPERATOR" ]
                li [ ClassName "nav-item nav-dropdown open" ] [
                    ul [ ClassName "nav-dropdown-items" ] [
                        a [ ClassName "nav-link"; Href "#" ] [
                            i [ ClassName "icon-puzzle" ] [ ]; text " Available actions"
                        ]
                    ]
                ]

                li [ ClassName "nav-title" ] [ text " INSPECTOR" ]
                li [ ClassName "nav-item nav-dropdown open" ] [
                    ul [ ClassName "nav-dropdown-items" ] [
                        a [ ClassName "nav-link"; Href "#" ] [
                            i [ ClassName "icon-puzzle" ] [ ]; text " Available actions"
                        ]
                    ]
                ]

                li [ ClassName "nav-title" ] [ text " LOAD TERMINAL" ]
                li [ ClassName "nav-item nav-dropdown open" ] [
                    ul [ ClassName "nav-dropdown-items" ] [
                        a [ ClassName "nav-link"; Href "#" ] [
                            i [ ClassName "icon-puzzle" ] [ ]; text " Available actions"
                        ]
                    ]
                ]

                li [ ClassName "nav-title" ] [ text " DISCHARGE TERMINAL" ]
                li [ ClassName "nav-item nav-dropdown open" ] [
                    ul [ ClassName "nav-dropdown-items" ] [
                        a [ ClassName "nav-link"; Href "#" ] [
                            i [ ClassName "icon-puzzle" ] [ ]; text " Available actions"
                        ]
                    ]
                ]

                li [ ClassName "nav-title" ] [ text " CAPTAIN" ]
                li [ ClassName "nav-item nav-dropdown open" ] [
                    ul [ ClassName "nav-dropdown-items" ] [
                        a [ ClassName "nav-link"; Href "#" ] [
                            i [ ClassName "icon-puzzle" ] [ ]; text " Available actions"
                        ]
                    ]
                ]
            ]
        ]
    ]


[<PassGenerics>]
let aside model dispatch =
    React.aside [ ClassName "aside-menu" ] [ text "Aside Menu" ]


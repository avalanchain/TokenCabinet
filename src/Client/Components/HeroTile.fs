module Client.HeroTile

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma

let safeComponents =
    let intersperse sep ls =
        List.foldBack (fun x -> function
            | [] -> [x]
            | xs -> x::sep::xs) ls []

    let components =
        [
            "Saturn", "https://saturnframework.github.io/docs/"
            "Fable", "http://fable.io"
            "Elmish", "https://elmish.github.io/elmish/"
            "Fulma", "https://mangelmaxime.github.io/Fulma"
            "Bulma\u00A0Templates", "https://dansup.github.io/bulma-templates/"
            "Fable.Remoting", "https://zaid-ajaj.github.io/Fable.Remoting/"
        ]
        |> List.map (fun (desc,link) -> a [ Href link ] [ str desc ] )
        |> intersperse (str ", ")
        |> span [ ]

    p [ ]
        [ strong [] [ str "SAFE Template" ]
          str " powered by: "
          components ]
let hero =
    Hero.hero [ Hero.Color IsInfo
                Hero.CustomClass "welcome" ]
        [ Hero.body [ ]
            [ Container.container [ ]
                [ Heading.h1 [ ]
                      [ str ("Purchase tokens " (*+ (string W3.version)*) ) ]
                  Heading.h4 [ Heading.IsSubtitle ]
                      [ safeComponents ] ] ] ]
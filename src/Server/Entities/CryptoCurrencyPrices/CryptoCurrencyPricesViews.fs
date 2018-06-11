namespace CryptoCurrencyPrices

open Microsoft.AspNetCore.Http
open Giraffe.GiraffeViewEngine
open Saturn

module Views =
  let index (ctx : HttpContext) (objs : CryptoCurrencyPrice list) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Listing CryptoCurrencyPrices"]

        table [_class "table is-hoverable is-fullwidth"] [
          thead [] [
            tr [] [
              th [] [rawText "Id"]
              th [] [rawText "Crypto Currency Name"]
              th [] [rawText "Price Usd"]
              th [] [rawText "Price Eth"]
              th [] [rawText "Price At"]
              th [] [rawText "Created On"]
              th [] [rawText "Created By"]
              th [] [rawText "Proof"]
              th [] []
            ]
          ]
          tbody [] [
            for o in objs do
              yield tr [] [
                td [] [rawText (string o.Id)]
                td [] [rawText (string o.CryptoCurrencyName)]
                td [] [rawText (string o.PriceUsd)]
                td [] [rawText (string o.PriceEth)]
                td [] [rawText (string o.PriceAt)]
                td [] [rawText (string o.CreatedOn)]
                td [] [rawText (string o.CreatedBy)]
                td [] [rawText (string o.Proof)]
                td [] [
                  a [_class "button is-text"; _href (Links.withId ctx (o.Id.ToString()) )] [rawText "Show"]
                  a [_class "button is-text"; _href (Links.edit ctx (o.Id.ToString()) )] [rawText "Edit"]
                  a [_class "button is-text is-delete"; attr "data-href" (Links.withId ctx (o.Id.ToString()) ) ] [rawText "Delete"]
                ]
              ]
          ]
        ]

        a [_class "button is-text"; _href (Links.add ctx )] [rawText "New CryptoCurrencyPrice"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])


  let show (ctx : HttpContext) (o : CryptoCurrencyPrice) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Show CryptoCurrencyPrice"]

        ul [] [
          li [] [ strong [] [rawText "Id: "]; rawText (string o.Id) ]
          li [] [ strong [] [rawText "CryptoCurrencyName: "]; rawText (string o.CryptoCurrencyName) ]
          li [] [ strong [] [rawText "PriceUsd: "]; rawText (string o.PriceUsd) ]
          li [] [ strong [] [rawText "PriceEth: "]; rawText (string o.PriceEth) ]
          li [] [ strong [] [rawText "PriceAt: "]; rawText (string o.PriceAt) ]
          li [] [ strong [] [rawText "CreatedOn: "]; rawText (string o.CreatedOn) ]
          li [] [ strong [] [rawText "CreatedBy: "]; rawText (string o.CreatedBy) ]
          li [] [ strong [] [rawText "Proof: "]; rawText (string o.Proof) ]
        ]
        a [_class "button is-text"; _href (Links.edit ctx (o.Id.ToString()))] [rawText "Edit"]
        a [_class "button is-text"; _href (Links.index ctx )] [rawText "Back"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let private form (ctx: HttpContext) (o: CryptoCurrencyPrice option) (validationResult : Map<string, string>) isUpdate =
    let validationMessage =
      div [_class "notification is-danger"] [
        a [_class "delete"; attr "aria-label" "delete"] []
        rawText "Oops, something went wrong! Please check the errors below."
      ]

    let field selector lbl key =
      div [_class "field"] [
        yield label [_class "label"] [rawText (string lbl)]
        yield div [_class "control has-icons-right"] [
          yield input [_class (if validationResult.ContainsKey key then "input is-danger" else "input"); _value (defaultArg (o |> Option.map selector) ""); _name key ; _type "text" ]
          if validationResult.ContainsKey key then
            yield span [_class "icon is-small is-right"] [
              i [_class "fas fa-exclamation-triangle"] []
            ]
        ]
        if validationResult.ContainsKey key then
          yield p [_class "help is-danger"] [rawText validationResult.[key]]
      ]

    let buttons =
      div [_class "field is-grouped"] [
        div [_class "control"] [
          input [_type "submit"; _class "button is-link"; _value "Submit"]
        ]
        div [_class "control"] [
          a [_class "button is-text"; _href (Links.index ctx)] [rawText "Cancel"]
        ]
      ]

    let cnt = [
      div [_class "container "] [
        form [ _action (if isUpdate then Links.withId ctx (o.Value.Id.ToString()) else Links.index ctx ); _method "post"] [
          if not validationResult.IsEmpty then
            yield validationMessage
          yield field (fun i -> (string i.Id)) "Id" "Id" 
          yield field (fun i -> (string i.CryptoCurrencyName)) "CryptoCurrencyName" "CryptoCurrencyName" 
          yield field (fun i -> (string i.PriceUsd)) "PriceUsd" "PriceUsd" 
          yield field (fun i -> (string i.PriceEth)) "PriceEth" "PriceEth" 
          yield field (fun i -> (string i.PriceAt)) "PriceAt" "PriceAt" 
          yield field (fun i -> (string i.CreatedOn)) "CreatedOn" "CreatedOn" 
          yield field (fun i -> (string i.CreatedBy)) "CreatedBy" "CreatedBy" 
          yield field (fun i -> (string i.Proof)) "Proof" "Proof" 
          yield buttons
        ]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let add (ctx: HttpContext) (o: CryptoCurrencyPrice option) (validationResult : Map<string, string>)=
    form ctx o validationResult false

  let edit (ctx: HttpContext) (o: CryptoCurrencyPrice) (validationResult : Map<string, string>) =
    form ctx (Some o) validationResult true

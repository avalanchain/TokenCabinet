namespace Wallets

open Microsoft.AspNetCore.Http
open Giraffe.GiraffeViewEngine
open Saturn

module Views =
  let index (ctx : HttpContext) (objs : WalletItem list) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Listing Wallets"]

        table [_class "table is-hoverable is-fullwidth"] [
          thead [] [
            tr [] [
              th [] [rawText "Id"]
              th [] [rawText "CustomerId"]
              th [] [rawText "Symbol"]
              th [] [rawText "AccType"]
              th [] [rawText "Address"]
              th [] [rawText "PubKey"]
              th [] [rawText "PrivKey"]
              th [] []
            ]
          ]
          tbody [] [
            for o in objs do
              yield tr [] [
                td [] [rawText (string o.Id)]
                td [] [rawText (string o.CustomerId)]
                td [] [rawText (string o.Symbol)]
                td [] [rawText (string o.AccType)]
                td [] [rawText (string o.Address)]
                td [] [rawText (string o.PubKey)]
                td [] [rawText (string o.PrivKey)]
                td [] [
                  a [_class "button is-text"; _href (Links.withId ctx o.Id )] [rawText "Show"]
                  a [_class "button is-text"; _href (Links.edit ctx o.Id )] [rawText "Edit"]
                  a [_class "button is-text is-delete"; attr "data-href" (Links.withId ctx o.Id ) ] [rawText "Delete"]
                ]
              ]
          ]
        ]

        a [_class "button is-text"; _href (Links.add ctx )] [rawText "New WalletItem"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])


  let show (ctx : HttpContext) (o : WalletItem) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Show WalletItem"]

        ul [] [
          li [] [ strong [] [rawText "Id: "]; rawText (string o.Id) ]
          li [] [ strong [] [rawText "CustomerId: "]; rawText (string o.CustomerId) ]
          li [] [ strong [] [rawText "Symbol: "]; rawText (string o.Symbol) ]
          li [] [ strong [] [rawText "AccType: "]; rawText (string o.AccType) ]
          li [] [ strong [] [rawText "Address: "]; rawText (string o.Address) ]
          li [] [ strong [] [rawText "PubKey: "]; rawText (string o.PubKey) ]
          li [] [ strong [] [rawText "PrivKey: "]; rawText (string o.PrivKey) ]
        ]
        a [_class "button is-text"; _href (Links.edit ctx o.Id)] [rawText "Edit"]
        a [_class "button is-text"; _href (Links.index ctx )] [rawText "Back"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let private form (ctx: HttpContext) (o: WalletItem option) (validationResult : Map<string, string>) isUpdate =
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
        form [ _action (if isUpdate then Links.withId ctx o.Value.Id else Links.index ctx ); _method "post"] [
          if not validationResult.IsEmpty then
            yield validationMessage
          yield field (fun i -> (string i.Id)) "Id" "Id" 
          yield field (fun i -> (string i.CustomerId)) "CustomerId" "CustomerId" 
          yield field (fun i -> (string i.Symbol)) "Symbol" "Symbol" 
          yield field (fun i -> (string i.AccType)) "AccType" "AccType" 
          yield field (fun i -> (string i.Address)) "Address" "Address" 
          yield field (fun i -> (string i.PubKey)) "PubKey" "PubKey" 
          yield field (fun i -> (string i.PrivKey)) "PrivKey" "PrivKey" 
          yield buttons
        ]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let add (ctx: HttpContext) (o: WalletItem option) (validationResult : Map<string, string>)=
    form ctx o validationResult false

  let edit (ctx: HttpContext) (o: WalletItem) (validationResult : Map<string, string>) =
    form ctx (Some o) validationResult true
namespace TokenSales

open Microsoft.AspNetCore.Http
open Giraffe.GiraffeViewEngine
open Saturn

module Views =
  let index (ctx : HttpContext) (objs : TokenSale list) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Listing TokenSales"]

        table [_class "table is-hoverable is-fullwidth"] [
          thead [] [
            tr [] [
              th [] [rawText "Id"]
              th [] [rawText "SaleTokenId"]
              th [] [rawText "SoftCapEth"]
              th [] [rawText "HardCapEth"]
              th [] [rawText "SoftCapUsd"]
              th [] [rawText "HardCapUsd"]
              th [] [rawText "Expectations"]
              th [] [rawText "StartDate"]
              th [] [rawText "EndDate"]
              th [] [rawText "CreatedOn"]
              th [] [rawText "CreatedBy"]
              th [] [rawText "Proof"]
              th [] []
            ]
          ]
          tbody [] [
            for o in objs do
              yield tr [] [
                td [] [rawText (string o.Id)]
                td [] [rawText (string o.Symbol)]
                td [] [rawText (string o.SoftCapEth)]
                td [] [rawText (string o.HardCapEth)]
                td [] [rawText (string o.SoftCapUsd)]
                td [] [rawText (string o.HardCapUsd)]
                td [] [rawText (string o.Expectations)]
                td [] [rawText (string o.StartDate)]
                td [] [rawText (string o.EndDate)]
                td [] [rawText (string o.CreatedOn)]
                td [] [rawText (string o.CreatedBy)]
                td [] [rawText (string o.Proof)]
                td [] [
                  a [_class "button is-text"; _href (Links.withId ctx (string o.Id) )] [rawText "Show"]
                  a [_class "button is-text"; _href (Links.edit ctx (string o.Id) )] [rawText "Edit"]
                  a [_class "button is-text is-delete"; attr "data-href" (Links.withId ctx (string o.Id) ) ] [rawText "Delete"]
                ]
              ]
          ]
        ]

        a [_class "button is-text"; _href (Links.add ctx )] [rawText "New TokenSale"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])


  let show (ctx : HttpContext) (o : TokenSale) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Show TokenSale"]

        ul [] [
          li [] [ strong [] [rawText "Id: "]; rawText (string o.Id) ]
          li [] [ strong [] [rawText "Symbol: "]; rawText (string o.Symbol) ]
          li [] [ strong [] [rawText "SoftCapEth: "]; rawText (string o.SoftCapEth) ]
          li [] [ strong [] [rawText "HardCapEth: "]; rawText (string o.HardCapEth) ]
          li [] [ strong [] [rawText "SoftCapUsd: "]; rawText (string o.SoftCapUsd) ]
          li [] [ strong [] [rawText "HardCapUsd: "]; rawText (string o.HardCapUsd) ]
          li [] [ strong [] [rawText "Expectations: "]; rawText (string o.Expectations) ]
          li [] [ strong [] [rawText "StartDate: "]; rawText (string o.StartDate) ]
          li [] [ strong [] [rawText "EndDate: "]; rawText (string o.EndDate) ]
          li [] [ strong [] [rawText "CreatedOn: "]; rawText (string o.CreatedOn) ]
          li [] [ strong [] [rawText "CreatedBy: "]; rawText (string o.CreatedBy) ]
          li [] [ strong [] [rawText "Proof: "]; rawText (string o.Proof) ]
        ]
        a [_class "button is-text"; _href (Links.edit ctx (string o.Id))] [rawText "Edit"]
        a [_class "button is-text"; _href (Links.index ctx )] [rawText "Back"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let private form (ctx: HttpContext) (o: TokenSale option) (validationResult : Map<string, string>) isUpdate =
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
        form [ _action (if isUpdate then Links.withId ctx (string o.Value.Id) else Links.index ctx ); _method "post"] [
          if not validationResult.IsEmpty then
            yield validationMessage
          yield field (fun i -> (string i.Id)) "Id" "Id" 
          yield field (fun i -> (string i.Symbol)) "Symbol" "Symbol" 
          yield field (fun i -> (string i.SoftCapEth)) "SoftCapEth" "SoftCapEth" 
          yield field (fun i -> (string i.HardCapEth)) "HardCapEth" "HardCapEth" 
          yield field (fun i -> (string i.SoftCapUsd)) "SoftCapUsd" "SoftCapUsd" 
          yield field (fun i -> (string i.HardCapUsd)) "HardCapUsd" "HardCapUsd" 
          yield field (fun i -> (string i.Expectations)) "Expectations" "Expectations" 
          yield field (fun i -> (string i.StartDate)) "StartDate" "StartDate" 
          yield field (fun i -> (string i.EndDate)) "EndDate" "EndDate" 
          yield field (fun i -> (string i.CreatedOn)) "CreatedOn" "CreatedOn" 
          yield field (fun i -> (string i.CreatedBy)) "CreatedBy" "CreatedBy" 
          yield field (fun i -> (string i.Proof)) "Proof" "Proof" 
          yield buttons
        ]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let add (ctx: HttpContext) (o: TokenSale option) (validationResult : Map<string, string>)=
    form ctx o validationResult false

  let edit (ctx: HttpContext) (o: TokenSale) (validationResult : Map<string, string>) =
    form ctx (Some o) validationResult true

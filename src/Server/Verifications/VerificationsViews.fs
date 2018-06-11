namespace Verifications

open System
open Microsoft.AspNetCore.Http
open Giraffe.GiraffeViewEngine
open Saturn

module Views =
  let inline format (guid: Guid) = guid.ToString("N")

  let index (ctx : HttpContext) (objs : Verification list) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Listing Verifications"]

        table [_class "table is-hoverable is-fullwidth"] [
          thead [] [
            tr [] [
              th [] [rawText "Id"]
              th [] [rawText "FirstName"]
              th [] [rawText "LastName"]
              th [] [rawText "MiddleName"]
              th [] [rawText "Gender"]
              th [] [rawText "DoB"]
              th [] [rawText "PassportNo"]
              th [] [rawText "PassportCountry"]
              th [] [rawText "RegistrationDate"]
              th [] [rawText "RegCountry"]
              th [] [rawText "Address"]
              th [] [rawText "City"]
              th [] [rawText "PostCode"]
              th [] [rawText "DocType"]
              th [] []
            ]
          ]
          tbody [] [
            for o in objs do
              yield tr [] [
                td [] [rawText (string o.Id)]
                td [] [rawText (string o.FirstName)]
                td [] [rawText (string o.LastName)]
                td [] [rawText (string o.MiddleName)]
                td [] [rawText (string o.Gender)]
                td [] [rawText (string o.DoB)]
                td [] [rawText (string o.PassportNo)]
                td [] [rawText (string o.PassportCountry)]
                td [] [rawText (string o.RegistrationDate)]
                td [] [rawText (string o.RegCountry)]
                td [] [rawText (string o.Address)]
                td [] [rawText (string o.City)]
                td [] [rawText (string o.PostCode)]
                td [] [rawText (string o.DocType)]
                td [] [
                  a [_class "button is-text"; _href (Links.withId ctx (format o.Id) )] [rawText "Show"]
                  a [_class "button is-text"; _href (Links.edit ctx (format o.Id) )] [rawText "Edit"]
                  a [_class "button is-text is-delete"; attr "data-href" (Links.withId ctx (format o.Id) ) ] [rawText "Delete"]
                ]
              ]
          ]
        ]

        a [_class "button is-text"; _href (Links.add ctx )] [rawText "New Verification"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])


  let show (ctx : HttpContext) (o : Verification) =
    let cnt = [
      div [_class "container "] [
        h2 [ _class "title"] [rawText "Show Verification"]

        ul [] [
          li [] [ strong [] [rawText "Id: "]; rawText (string o.Id) ]
          li [] [ strong [] [rawText "FirstName: "]; rawText (string o.FirstName) ]
          li [] [ strong [] [rawText "LastName: "]; rawText (string o.LastName) ]
          li [] [ strong [] [rawText "MiddleName: "]; rawText (string o.MiddleName) ]
          li [] [ strong [] [rawText "Gender: "]; rawText (string o.Gender) ]
          li [] [ strong [] [rawText "DoB: "]; rawText (string o.DoB) ]
          li [] [ strong [] [rawText "PassportNo: "]; rawText (string o.PassportNo) ]
          li [] [ strong [] [rawText "PassportCountry: "]; rawText (string o.PassportCountry) ]
          li [] [ strong [] [rawText "RegistrationDate: "]; rawText (string o.RegistrationDate) ]
          li [] [ strong [] [rawText "RegCountry: "]; rawText (string o.RegCountry) ]
          li [] [ strong [] [rawText "Address: "]; rawText (string o.Address) ]
          li [] [ strong [] [rawText "City: "]; rawText (string o.City) ]
          li [] [ strong [] [rawText "PostCode: "]; rawText (string o.PostCode) ]
          li [] [ strong [] [rawText "DocType: "]; rawText (string o.DocType) ]
        ]
        a [_class "button is-text"; _href (Links.edit ctx (format o.Id))] [rawText "Edit"]
        a [_class "button is-text"; _href (Links.index ctx )] [rawText "Back"]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let private form (ctx: HttpContext) (o: Verification option) (validationResult : Map<string, string>) isUpdate =
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
        form [ _action (if isUpdate then Links.withId ctx (format o.Value.Id) else Links.index ctx ); _method "post"] [
          if not validationResult.IsEmpty then
            yield validationMessage
          yield field (fun i -> (string i.Id)) "Id" "Id" 
          yield field (fun i -> (string i.FirstName)) "FirstName" "FirstName" 
          yield field (fun i -> (string i.LastName)) "LastName" "LastName" 
          yield field (fun i -> (string i.MiddleName)) "MiddleName" "MiddleName" 
          yield field (fun i -> (string i.Gender)) "Gender" "Gender" 
          yield field (fun i -> (string i.DoB)) "DoB" "DoB" 
          yield field (fun i -> (string i.PassportNo)) "PassportNo" "PassportNo" 
          yield field (fun i -> (string i.PassportCountry)) "PassportCountry" "PassportCountry" 
          yield field (fun i -> (string i.RegistrationDate)) "RegistrationDate" "RegistrationDate" 
          yield field (fun i -> (string i.RegCountry)) "RegCountry" "RegCountry" 
          yield field (fun i -> (string i.Address)) "Address" "Address" 
          yield field (fun i -> (string i.City)) "City" "City" 
          yield field (fun i -> (string i.PostCode)) "PostCode" "PostCode" 
          yield field (fun i -> (string i.DocType)) "DocType" "DocType" 
          yield buttons
        ]
      ]
    ]
    App.layout ([section [_class "section"] cnt])

  let add (ctx: HttpContext) (o: Verification option) (validationResult : Map<string, string>)=
    form ctx o validationResult false

  let edit (ctx: HttpContext) (o: Verification) (validationResult : Map<string, string>) =
    form ctx (Some o) validationResult true

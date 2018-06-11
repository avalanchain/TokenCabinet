namespace CustomerPreferences

[<CLIMutable>]
type CustomerPreference = {
  Id: System.Guid
  Language: string
}


module Validation =
  let supportedLangs = [ "English" ]

  let validate v =
    let validators = [
      fun u -> if u.Id = System.Guid.Empty then Some ("Id", "Id shouldn't be empty") else None
      fun u -> if isNull u.Language then Some ("Language", "Language shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

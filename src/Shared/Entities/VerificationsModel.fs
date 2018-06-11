namespace Verifications

open System

[<CLIMutable>]
type Verification = {
  Id: System.Guid
  FirstName: string
  LastName: string
  MiddleName: string
  Gender: string
  DoB: System.DateTime
  PassportNo: string
  PassportCountry: string
  RegistrationDate: System.DateTime
  RegCountry: string
  Address: string
  City: string
  PostCode: string
  DocType: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if u.Id = Guid.Empty then Some ("Id", "Id shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

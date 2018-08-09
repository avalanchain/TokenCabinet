namespace PwdResetTokenInfos

open System

[<CLIMutable>]
type PwdResetTokenInfo = {
  PwdResetToken: string
  CustomerId: string
  Issued: System.DateTime
  Expires: System.DateTime
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if String.IsNullOrWhiteSpace u.PwdResetToken then Some ("PwdResetToken", "PwdResetToken shouldn't be empty") else None
      fun u -> if String.IsNullOrWhiteSpace u.CustomerId    then Some ("CustomerId", "CustomerId shouldn't be empty") else None
      fun u -> if u.Issued  <> new DateTime() then Some ("Issued", "Issued shouldn't be empty") else None
      fun u -> if u.Expires <> new DateTime() then Some ("Expires", "Expires shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

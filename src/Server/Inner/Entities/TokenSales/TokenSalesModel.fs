namespace TokenSales
open System

[<CLIMutable>]
type TokenSale = {
  Id: int
  Symbol      : string
  SoftCapEth  : decimal
  HardCapEth  : decimal
  SoftCapUsd  : decimal
  HardCapUsd  : decimal
  Expectations: decimal
  StartDate   : DateTime
  EndDate     : DateTime
  CreatedOn   : DateTime
  CreatedBy   : string
  Proof       : string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if isNull u.Symbol then Some ("Symbol", "'Symbol' shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

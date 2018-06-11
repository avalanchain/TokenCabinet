namespace TokenSales

[<CLIMutable>]
type TokenSale = {
  Id: int
  SaleTokenId: string
  SoftCapEth: decimal
  HardCapEth: decimal
  SoftCapUsd: decimal
  HardCapUsd: decimal
  Expectations: decimal
  StartDate: System.DateTime
  EndDate: System.DateTime
  CreatedOn: System.DateTime
  CreatedBy: System.DateTime
  Proof: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if isNull u.SaleTokenId then Some ("SaleTokenId", "'Sale Token Id' shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

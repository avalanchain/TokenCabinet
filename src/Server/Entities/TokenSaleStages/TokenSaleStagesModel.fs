namespace TokenSaleStages

[<CLIMutable>]
type TokenSaleStage = {
  Id: int
  TokenSaleId: int
  CapEth: decimal
  CapUsd: decimal
  StartDate: System.DateTime
  EndDate: System.DateTime
  CreatedOn: System.DateTime
  CreatedBy: System.DateTime
  Proof: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if u.TokenSaleId = 0 then Some ("SaleTokenId", "'Sale Token Id' shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

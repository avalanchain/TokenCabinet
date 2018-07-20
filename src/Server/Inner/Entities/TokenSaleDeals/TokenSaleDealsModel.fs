namespace TokenSaleDeals

[<CLIMutable>]
type TokenSaleDeal = {
  Id: int
  SaleTokenId: string
  PriceUsd: decimal
  PriceEth: decimal
  BonusPercent: decimal
  BonusTokens: decimal
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

namespace TokenSaleStatuses

[<CLIMutable>]
type TokenSaleStatus = {
  Id: int
  TokenSaleId: int
  TokenSaleStatus: string
  ActiveStageId: int
  SaleTokenId: int
  PriceUsd: decimal
  PriceEth: decimal
  BonusPercent: decimal
  BonusTokens: decimal
  StartDate: System.DateTime
  EndDate: System.DateTime
  CreatedOn: System.DateTime
  CreatedBy: string
  Proof: string
}

module Validation =
  let validate v =
    let validators = [    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

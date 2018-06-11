namespace Investments

[<CLIMutable>]
type Investment = {
  Id: System.Guid
  CustomerId: System.Guid
  EventType: string
  TokenSaleDealId: int
  Date: System.DateTime
  AmountEth: decimal
  AmountTokens: decimal
  Rate: decimal
  InvestmentTranStatusId: int
  CreatedOn: System.DateTime
  CreatedBy: System.DateTime
  Proof: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if u.Id = System.Guid.Empty then Some ("Id", "'Id' shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

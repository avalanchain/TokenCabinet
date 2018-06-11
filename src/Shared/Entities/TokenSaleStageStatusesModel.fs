namespace TokenSaleStageStatuses

[<CLIMutable>]
type TokenSaleStageStatus = {
  Id: int
  TokenSaleStageId: int
  Status: int
  CreatedOn: System.DateTime
  CreatedBy: System.DateTime
  Proof: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if u.TokenSaleStageId = 0 then Some ("TokenSaleStageId", "'Token Sale Stage Id' shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

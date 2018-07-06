namespace WalletsKV

[<CLIMutable>]
type WalletKV = {
  CustomerId: string
  Wallet: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if isNull u.CustomerId then Some ("CustomerId", "CustomerId shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

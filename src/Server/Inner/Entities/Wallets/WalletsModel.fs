namespace Wallets

[<CLIMutable>]
type WalletItem = {
  Id: string
  CustomerId: string
  Symbol: string
  AccType: string
  Address: string
  PubKey: string
  PrivKey: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if isNull u.Id then Some ("Id", "Id shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

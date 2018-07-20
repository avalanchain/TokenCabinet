namespace CryptoCurrencyPrices

open System
[<CLIMutable>]
type CryptoCurrencyPrice = {
  Id: int
  CryptoCurrencyName: string
  PriceUsd: decimal
  PriceEth: decimal
  PriceAt: System.DateTime
  CreatedOn: System.DateTime
  CreatedBy: System.DateTime
  Proof: string
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if isNull u.CryptoCurrencyName then Some ("CryptoCurrencyName", "'Crypto Currency Name' shouldn't be empty") else None
      fun u -> if u.PriceUsd <= 0M then Some ("PriceUsd", "'Price Usd' should be positive") else None
      fun u -> if u.PriceEth <= 0M then Some ("PriceEth", "'Price Eth' should be positive") else None
      fun u -> if u.PriceAt = DateTime() then Some ("PriceAt", "'Price At' shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty

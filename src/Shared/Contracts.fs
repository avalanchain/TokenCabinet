namespace ServerCode

module Contracts = 
    open System
    open System.Collections.Concurrent
    open Commodities 

    //let private contracts = new ConcurrentDictionary<uint32, SellerContract>()
    let mutable private contracts = Map.empty<uint32, SellerContract>

    let getContracts() = contracts |> Seq.map(fun kv -> kv.Key, kv.Value) |> Map.ofSeq
    let setContract uid contract =
        printfn "UID: %d" uid
        contracts <- contracts.Add(uid, contract)
        //contracts.AddOrUpdate(uid, fun _ -> contract, new Func<_,_,>(fun (a, b) -> contract)) |> ignore

    ///// ----- Show states
    let states() = contracts |> Seq.sortBy (fun kv -> kv.Key) |> Seq.map (fun kv -> (kv.Key, kv.Value.State))

module Customer.Transactions

open Nethereum
open System.Collections.Generic
open FSharp.Control.Tasks
open Shared.WalletPublic
open Wallet

open Shared.ViewModels

open System.Numerics
open Nethereum.Hex.HexTypes
open Customer

type ETransactions = {
    ETransactions: ETransaction list
}

let hostLink = "http://localhost:8545"
let checkETHNet (eAccount: EthAccount) = 
    let (CCPrivKey privateKey) = eAccount.EPrivKey
    let acc = new Nethereum.Web3.Accounts.Account(privateKey)
    let web3 = new Nethereum.Web3.Web3(acc, hostLink)
    web3.Eth.Transactions <> null

let getTransactions (eAccount: EthAccount) checkETHNet = task {
    let transactions = new List<ETransaction>()
    if checkETHNet
    then
        // printfn "in here3"
        let (CCPrivKey privateKey) = eAccount.EPrivKey
        let acc = new Nethereum.Web3.Accounts.Account(privateKey)
        let web3 = new Nethereum.Web3.Web3(acc, hostLink)
        let test = "0xf34b475aa2efcd445e6c1dc54be37d95bfa2a686"
        let! count = web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(test)//eAccount.EAddress.Value) 
        printfn "count %A" (count.Value.ToString())
        let! endBlockNumber = web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()
        printfn "endBlockNumber %A" (endBlockNumber.Value.ToString())
        if count.Value <> BigInteger.Zero 
        then  
            let! endBlockNumber = web3.Eth.Blocks.GetBlockNumber.SendRequestAsync() //getBlockNumber()
            let startBlockNumber  = BigInteger.Zero |> HexBigInteger
            
            // printfn "blocks %A" (endBlockNumber.Value.ToString())
            for i in startBlockNumber.Value .. endBlockNumber.Value do
                let t = i |> HexBigInteger
                let blockParameter = new Nethereum.RPC.Eth.DTOs.BlockParameter(t)

                let! block = web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockParameter) //w3.eth.getBlock(i |> U4.Case4, true)
                        
                if not (isNull block) && not (isNull block.Transactions)
                then 
                    for t in block.Transactions do
                        if t.To.ToLowerInvariant() = eAccount.EAddress.Value.ToLowerInvariant()
                        then
                            let (et: ETransaction) = {
                                TransactionHash  = t.TransactionHash
                                TransactionIndex = string t.TransactionIndex.Value
                                BlockHash        = string t.BlockHash
                                BlockNumber      = string t.BlockNumber.Value
                                From             = string t.From
                                To               = string t.To
                                Gas              = string t.Gas.Value
                                GasPrice         = string t.GasPrice.Value
                                Value            = decimal t.Value.Value
                            } 
                            transactions.Add(et)
        let tss:ETransaction list = []
        printfn "privateKey %A" eAccount.EAddress
        // printfn "blocks %A" endBlockNumber.Value
        printfn "transactionsLength %A" transactions.Count
    // let tt = transactions |> List.ofSeq |> ETransactions 
    return transactions |> List.ofSeq 
}
module Customer.Transactions

open System
open System.Collections.Generic
open System.Numerics
open System.Threading.Tasks

open FSharp.Control.Tasks
open Shared.WalletPublic
open Wallet

open Shared.ViewModels

open Nethereum
open Nethereum.Hex.HexTypes
open Customer
open System.Numerics
open Customer

type ETransactions = {
    EAccount                    : CCAddress
    RemainingTransactionCount   : BigInteger
    RemainingBalance            : BigInteger
    ETransactions               : ETransaction list
}

let hostLink = "http://localhost:8545"
let checkETHNet (eAccount: EthAccount) = 
    let web3 = new Nethereum.Web3.Web3(hostLink)
    web3.Eth.Transactions <> null

let getTransactions2 (accounts: CCAddress seq) = task {
    let web3 = new Nethereum.Web3.Web3(hostLink)
    let! endBlockNumber = web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()
    let mutable accountTransactions = Map.empty<CCAddress, ETransactions>
    for CCAddress address in accounts do
        let! remainingTransactionCount  = web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(address, endBlockNumber)
        let! remainingBalance           = web3.Eth.GetBalance.SendRequestAsync(address, endBlockNumber)
        let transactions = {EAccount = (CCAddress address)
                            RemainingTransactionCount = remainingTransactionCount.Value
                            RemainingBalance          = remainingBalance.Value
                            ETransactions             = [] } 
        accountTransactions <- accountTransactions.Add ((CCAddress address), transactions)

    let checkAnyRemaining (accountTransactions: Map<CCAddress, ETransactions>) =
        accountTransactions
        |> Seq.exists (fun at -> at.Value.RemainingBalance > BigInteger.Zero 
                                || at.Value.RemainingTransactionCount > BigInteger.Zero)

    let mutable blockNumber = endBlockNumber
    while blockNumber.Value >= BigInteger.Zero && checkAnyRemaining accountTransactions do 
        let! block = web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber)
        if block |> isNull |> not then 
            for t in block.Transactions do
                if t.From <> t.To then 
                    match accountTransactions |> Map.tryFind (CCAddress t.From) with
                    | Some at -> 
                        let newAt = { at with   RemainingTransactionCount   = at.RemainingTransactionCount - BigInteger(1)
                                                RemainingBalance            = at.RemainingBalance + t.Value.Value }
                        accountTransactions <- accountTransactions |> Map.add (CCAddress t.From) newAt
                    | None -> ()

                    match accountTransactions |> Map.tryFind (CCAddress t.To) with
                    | Some at -> 
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
                        let newAt = { at with   RemainingBalance    = at.RemainingBalance - t.Value.Value
                                                ETransactions       = et :: at.ETransactions }
                        accountTransactions <- accountTransactions |> Map.add (CCAddress t.From) newAt
                    | None -> ()
    
    return accountTransactions
}

let getTransactions (eAccount: EthAccount) checkETHNet = task {
    let transactions = ResizeArray<ETransaction>()
    if checkETHNet
    then
        // printfn "in here3"
        let (CCAddress address) = eAccount.EAddress
        //let acc = new Nethereum.Web3.Accounts.Account(privateKey)
        let web3 = new Nethereum.Web3.Web3(hostLink)
        // let test = "0xf34b475aa2efcd445e6c1dc54be37d95bfa2a686"
        let blockNumber = web3.Eth.Blocks.GetBlockNumber
        let! count = web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(address)//eAccount.EAddress.Value) 
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
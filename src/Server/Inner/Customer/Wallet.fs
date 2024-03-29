module Customer.Wallet

open Shared.WalletPublic

open Nethereum
open Nethereum.Hex.HexConvertors.Extensions

type EthAccount = {
    EAddress: CCAddress
    EPubKey:  CCPubKey
    EPrivKey: CCPrivKey
}   with member __.PublicPart = {   Address = __.EAddress
                                    PubKey  = __.EPubKey }

type BtcAccount = {
    BAddress: CCAddress
    BPubKey:  CCPubKey
    BPrivKey: CCPrivKey
}   with member __.PublicPart = {   Address = __.BAddress
                                    PubKey  = __.BPubKey }

type NetworkEnvAccounts = {
    Eth:  EthAccount
    Etc:  EthAccount
    Btc:  BtcAccount
    Ltc:  BtcAccount
    Btg:  BtcAccount
    Bch:  BtcAccount
    Dash: BtcAccount
}   with member __.PublicPart: NetworkEnvPublicPart = { Eth  = __.Eth.PublicPart
                                                        Etc  = __.Etc.PublicPart
                                                        Btc  = __.Btc.PublicPart
                                                        Ltc  = __.Ltc.PublicPart
                                                        Btg  = __.Btg.PublicPart
                                                        Bch  = __.Bch.PublicPart
                                                        Dash = __.Dash.PublicPart }
type NetworkEnv = MainEnv | TestEnv

type Wallet = {
    CustomerId: System.Guid
    Test: NetworkEnvAccounts
    Main: NetworkEnvAccounts
}   with member __.PublicPart (env: NetworkEnv): WalletPublicPart = 
            {   CustomerId  = __.CustomerId
                Accounts    = match env with    
                                | TestEnv -> __.Test.PublicPart
                                | MainEnv -> __.Main.PublicPart }


let masterEncryptionKey = "<GetFromConfig>" // Load from configuration 

let createCustomerWallet id =
    let ethEcKey      = Signer.EthECKey.GenerateKey()
    let eAccount =
        {   EAddress = ethEcKey.GetPublicAddress() |> CCAddress
            EPubKey  = ethEcKey.GetPubKey().ToHex() |> CCPubKey 
            EPrivKey = ethEcKey.GetPrivateKey() |> CCPrivKey }  // TODO: Encrypt
    
    let btcKey          = new NBitcoin.ExtKey()
    let bAccount network =
        {   BAddress = btcKey.PrivateKey.PubKey.GetAddress(network).ToString() |> CCAddress
            BPubKey  = btcKey.GetWif(network).ToString() |> CCPubKey
            BPrivKey = btcKey.PrivateKey.GetBitcoinSecret(network).ToString() |> CCPrivKey }
            // BPrivKey = btcKey.PrivateKey.GetEncryptedBitcoinSecret(masterEncryptionKey, network).ToString() |> CCPrivKey }
    let mainEnv = { Eth  = eAccount
                    Etc  = eAccount
                    Btc  = bAccount NBitcoin.Network.Main
                    Ltc  = bAccount NBitcoin.Altcoins.Litecoin.Instance.Mainnet
                    Btg  = bAccount NBitcoin.Altcoins.BGold.Instance.Mainnet
                    Bch  = bAccount NBitcoin.Altcoins.BCash.Instance.Mainnet
                    Dash = bAccount NBitcoin.Altcoins.Dash.Instance.Mainnet  }
    let testEnv = { Eth  = eAccount
                    Etc  = eAccount
                    Btc  = bAccount NBitcoin.Network.TestNet
                    Ltc  = bAccount NBitcoin.Altcoins.Litecoin.Instance.Testnet
                    Btg  = bAccount NBitcoin.Altcoins.BGold.Instance.Testnet
                    Bch  = bAccount NBitcoin.Altcoins.BCash.Instance.Testnet
                    Dash = bAccount NBitcoin.Altcoins.Dash.Instance.Testnet  }

    {   CustomerId  = id
        Main = mainEnv
        Test = testEnv }


type MType = 
    | A of int


let f (t: MType) = 
    let mtype = A 2
    let (A inner) = mtype
    inner

// let getTransactions eAccount =

// let addAccountToNet eAccount = 
//     //  Nethereum.Web3.Web3(account)
//     let (CCPrivKey privateKey) = eAccount.EPrivKey
//     let acc = new Nethereum.Web3.Accounts.Account(privateKey)
    
    // let web3 = new Nethereum.Web3.Web3(acc, "http://localhost:8545")
    // web3.Eth.
    // web3.Personal.NewAccount.
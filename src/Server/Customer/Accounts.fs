module Customer.Accounts

open Nethereum
open Nethereum.Hex.HexConvertors.Extensions

type PublicAccountPart = {
    CustomerId: System.Guid
    EthAddress: string
    BtcAddress: string
}

type PrivateAccountPart = {
    CustomerId: System.Guid
    EthAddress: string
    EthPubKey:  string
    EthPrivKey: string
    BtcAddress: string
    BtcPubKey:  string
    BtcPrivKey: string
}


let masterEncryptionKey = "<GetFromConfig>" // Load from configuration 

let createCustomerAccounts id = 
    let ethEcKey      = Signer.EthECKey.GenerateKey()
    let ethPubKey     = ethEcKey.GetPubKey().ToHex()  // TODO: Encrypt
    let ethPrivateKey = ethEcKey.GetPrivateKeyAsBytes().ToHex()
    
    let btcKey          = new NBitcoin.ExtKey()
    let btcAddress      = btcKey.PrivateKey.PubKey.GetAddress(NBitcoin.Network.TestNet).ToString()
    let btcPubKey       = btcKey.GetWif(NBitcoin.Network.TestNet).ToString()
    let btcPrivateKey   = btcKey.PrivateKey.GetWif(NBitcoin.Network.TestNet).ToString()

    {   CustomerId  = id
        EthAddress  = ethEcKey.GetPublicAddress()
        EthPubKey   = ethPubKey
        EthPrivKey  = ethPrivateKey
        BtcAddress  = btcAddress
        BtcPubKey   = btcPubKey
        BtcPrivKey  = btcPrivateKey 
    }
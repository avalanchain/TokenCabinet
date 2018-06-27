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
    EthKey: string
    BtcAddress: string
    BtcKey: string
}


let masterEncryptionKey = "<GetFromConfig>" // Load from configuration 

let createCustomerAccounts id = 
    let ethEcKey = Signer.EthECKey.GenerateKey()
    let ethPrivateKey = ethEcKey.GetPrivateKeyAsBytes().ToHex()  // TODO: Encrypt
    
    let btcKey          = new NBitcoin.Key()
    let btcAddress      = btcKey.PubKey.GetAddress(NBitcoin.Network.TestNet)
    let btcPrivateKey   = btcKey.GetBitcoinSecret(NBitcoin.Network.TestNet)

    {   CustomerId  = id
        EthAddress  = ethEcKey.GetPublicAddress()
        EthKey      = ethPrivateKey
        BtcAddress  = "" //btcAddress.ToString()
        BtcKey      = "" 
    }
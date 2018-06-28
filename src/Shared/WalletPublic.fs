module Customer.WalletPublic

type CCAddress = CCAddress of string
type CCPubKey  = CCPubKey  of string
type CCPrivKey = CCPrivKey of string

type EthAccountPublicPart = {
    EAddress: CCAddress
    EPubKey:  CCPubKey
}

type BtcAccountPublicPart = {
    BAddress: CCAddress
    BPubKey:  CCPubKey
}

type NetworkEnvPublicPart = {
    Eth:  EthAccountPublicPart
    Etc:  EthAccountPublicPart
    Btc:  BtcAccountPublicPart
    Ltc:  BtcAccountPublicPart
    Btg:  BtcAccountPublicPart
    Bth:  BtcAccountPublicPart
    Dash: BtcAccountPublicPart
}

type WalletPublicPart = {
    CustomerId: System.Guid
    Accounts: NetworkEnvPublicPart
}


namespace Shared

open System
open Auth

type Counter = int
 
type ServerError =
    | AuthError of AuthError
    | InternalError of exn
    | NotImplementedError

type ServerResult<'T> = Result<'T, ServerError>

type CryptoCurrencySymbol = ETH | ETC | BTC | LTC | BCH | BTG | DASH

module WalletPublic =

    type CCAddress = CCAddress of string
    type CCPubKey  = CCPubKey  of string
    type CCPrivKey = CCPrivKey of string

    type AccountPublicPart = {
        Address: CCAddress
        PubKey:  CCPubKey
    }

    type NetworkEnvPublicPart = {
        Eth:  AccountPublicPart
        Etc:  AccountPublicPart
        Btc:  AccountPublicPart
        Ltc:  AccountPublicPart
        Btg:  AccountPublicPart
        Bch:  AccountPublicPart
        Dash: AccountPublicPart
    }

    type WalletPublicPart = {
        CustomerId: System.Guid
        Accounts: NetworkEnvPublicPart
    } with member __.ForSymbol symbol = match symbol with   
                                        | ETH  -> __.Accounts.Eth 
                                        | ETC  -> __.Accounts.Etc
                                        | BTC  -> __.Accounts.Btc
                                        | LTC  -> __.Accounts.Ltc
                                        | BCH  -> __.Accounts.Bch
                                        | BTG  -> __.Accounts.Btg
                                        | DASH -> __.Accounts.Dash



module ViewModels = 
    type TokenSale = {
        SaleToken: SaleToken

        SoftCapEth: decimal
        HardCapEth: decimal
        SoftCapUsd: decimal
        HardCapUsd: decimal
        Expectations: decimal
        StartDate: System.DateTime
        EndDate: System.DateTime
        
        TokenSaleState: TokenSaleState

        TokenSaleStages: TokenSaleStage list
    }
    and SaleToken = {
        Symbol: string
        Name: string
        LogoUrl: string
        TotalSupply: decimal
    }
    and TokenSaleState = {
        TokenSaleStatus: TokenSaleStatus
        ActiveStage: TokenSaleStage
        PriceUsd: decimal
        PriceEth: decimal
        BonusPercent: decimal
        BonusTokens: decimal
        StartDate: System.DateTime
        EndDate: System.DateTime
    }
    and TokenSaleStage = {
        Id: int
        Name: string
        CapEth: decimal
        CapUsd: decimal
        StartDate: System.DateTime
        EndDate: System.DateTime
        Status: TokenSaleStageStatus
    }
    and [<RequireQualifiedAccess>] TokenSaleStatus = 
        | NotStarted
        | Active
        | TokenDistribution
        | Completed
        | Paused
        | Suspended
        
    and [<RequireQualifiedAccess>] TokenSaleStageStatus =
        | Expectation
        | Active
        | Completed
        | Cancelled
        | Paused

    type Customer = {
        Id: System.Guid
        FirstName: string
        LastName: string
        EthAddress: string
        Password: string
        PasswordSalt: string
        Avatar: string
        Email: string
    }
    type FullCustomer = {
        Customer            : Customer
        IsVerified          : bool
        VerificationEvent   : CustomerVerificationEvents.CustomerVerificationEvent option
        CustomerTier        : CustomerTier
        CustomerPreference  : CustomerPreferences.CustomerPreference
        Wallet              : WalletPublic.WalletPublicPart
    }
    and CustomerTier = 
        | Tier1
        | Tier2
        | Tier3
    and InvestmentHistory = InvestmentHistoryItem list
    and InvestmentHistoryItem = {
        Date: DateTime
        AmountTokens: decimal
        PricePaidUsd: decimal
        PricePaidEth: decimal
        TokenSaleDeal: TokenSaleDeals.TokenSaleDeal
        Status: InvestmentHistoryItemStatus
    }
    and InvestmentHistoryItemStatus =
        | Expected
        | Transferred
    and Referals = Referal list
    and Referal = {
        DateRegistered: DateTime
        FullName: string
        Email: string
    } 

   
    type CryptoCurrency = {
        Symbol    : CryptoCurrencySymbol
        Name      : string
        LogoUrl   : string
    }

    type CryptoCurrencyPrice = {
        Symbol: CryptoCurrencySymbol
        CryptoCurrencyName: string
        PriceUsd: decimal
        PriceEth: decimal
        PriceAt: System.DateTime
    }

    type CurrencyPriceTick = { Prices: CryptoCurrencyPrice list } 

module Route =
    /// Defines how routes are generated on server and mapped from client
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

/// A type that specifies the communication protocol for client and server
/// Every record field must have the type : 'a -> Async<'b> where 'a can also be `unit`
/// Add more such fields, implement them on the server and they be directly available on client
type IAdminProtocol = {   
    getInitCounter      : unit -> Async<ServerResult<Counter>>
    initDb              : unit -> Async<ServerResult<unit>> 
}

type ITokenSaleProtocol = {
    login               : LoginInfo -> Async<ServerResult<AuthToken>>
    getCryptoCurrencies : unit -> Async<ServerResult<ViewModels.CryptoCurrency list>> 

    getTokenSale        : unit -> Async<ServerResult<ViewModels.TokenSale>> 
    getFullCustomer     : SecureVoidRequest -> Async<ServerResult<ViewModels.FullCustomer>> 

    getPriceTick        : uint64 -> Async<ServerResult<ViewModels.CurrencyPriceTick>>
}

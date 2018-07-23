module Client.Cabinet

open Shared

type MenuPage =
    | PurchaseToken
    | Investments
    | ReferralProgram
    | Verification
    | Contacts
    // | Dashboard
    with static member Default = PurchaseToken  


type Msg =
    | VerificationMsg   of VerificationMsg
    | PurchaseTokenMsg  of PurchaseTokenMsg
    | InvestmentsMsg    of InvestmentsMsg
    | ReferralProgramMsg
    | ContactsMsg
    | DashboardMsg
    | ServerMsg         of ServerMsg
and ServerMsg =
    | GetCryptoCurrenciesCompleted  of ViewModels.CryptoCurrency list
    | GetTokenSaleCompleted         of ViewModels.TokenSale
    | GetFullCustomerCompleted      of ViewModels.FullCustomer
    | PriceTick                     of ViewModels.CurrencyPriceTick
    | GetTransactionsCompleted      of ViewModels.ETransaction list
and PurchaseTokenMsg = 
    | ActiveSymbolChanged of symbol: CryptoCurrencySymbol
    | CCAmountChanges     of decimal
    | TAmountChanges      of decimal
    | AddressCopied       of string
    | BuyTokens           //of decimal
    | SignResult          of Result<string, string>
and VerificationMsg = 
    | TabChanged          of int
and InvestmentsMsg = 
    | GetCoinbase          //of string
    | GetTransactions     of ViewModels.ETransaction list
    | GetTransactionCount
    | CoinbaseResult           of Result<string, string>
    | TransactionsResult       of Result<Web3Types.Transaction list, string>
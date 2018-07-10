module Client.CabinetModel 

open Shared
open LoginCommon
open Shared.WalletPublic


type Msg =
    | VerificationMsg of VerificationMsg
    | PurchaseTokenMsg of PurchaseTokenMsg
    | MyInvestmentsMsg
    | ReferralProgramMsg
    | ContactsMsg
    | DashboardMsg
    | ServerMsg     of ServerMsg
and ServerMsg =
    | GetCryptoCurrenciesCompleted  of ViewModels.CryptoCurrency list
    | GetTokenSaleCompleted         of ViewModels.TokenSale
    | GetFullCustomerCompleted      of ViewModels.FullCustomer
    | PriceTick                     of ViewModels.CurrencyPriceTick
and PurchaseTokenMsg = 
    | ActiveSymbolChanged of symbol: CryptoCurrencySymbol
    | CCAmountChanges     of decimal
    | TAmountChanges      of decimal
    | AddressCopied       of string
and VerificationMsg = 
    | TabChanged          of int

type Model = {
    Auth                   : AuthModel
    // Loading     : bool  
    CryptoCurrencies       : ViewModels.CryptoCurrency list
    CurrenciesCurentPrices : ViewModels.CurrencyPriceTick
    ActiveSymbol           : CryptoCurrencySymbol

    TokenSale              : ViewModels.TokenSale option
    FullCustomer           : ViewModels.FullCustomer option

    PurchaseTokenModel     : PurchaseTokenModel
    VerificationModel     : VerifiacationModel
}
and PurchaseTokenModel = {
    CCTokens               : decimal
    BuyTokens              : decimal
    CCAddress              : string
    TotalPrice             : decimal
} 

and VerifiacationModel = {
    CurrentTab             : int
} 

module Client.CabinetModel 

open Shared
open LoginCommon
open Shared.WalletPublic
open Elmish.Toastr
open Fable.PowerPack
open System.Transactions
open Web3Types
open System.Transactions


type Msg =
    | VerificationMsg   of VerificationMsg
    | PurchaseTokenMsg  of PurchaseTokenMsg
    | InvestmentsMsg of InvestmentsMsg
    | ReferralProgramMsg
    | ContactsMsg
    | DashboardMsg
    | ServerMsg         of ServerMsg
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
    | BuyTokens           //of decimal
    | SignResult          of Result<string, string>
and VerificationMsg = 
    | TabChanged          of int
and InvestmentsMsg = 
    | GetCoinbase          //of string
    | GetTransactions     of string
    | GetTransactionCount
    | CoinbaseResult           of Result<string, string>
    | TransactionsResult       of Result<Web3Types.Transaction list, string>

type Model = {
    Auth                   : AuthModel
    // Loading     : bool  
    CryptoCurrencies       : ViewModels.CryptoCurrency list
    CurrenciesCurentPrices : ViewModels.CurrencyPriceTick
    ActiveSymbol           : CryptoCurrencySymbol

    TokenSale              : ViewModels.TokenSale option
    FullCustomer           : ViewModels.FullCustomer option

    PurchaseTokenModel     : PurchaseTokenModel
    VerificationModel      : VerificationModel
    InvestmentModel        : InvestmentModel

    IsWeb3                 : bool
}
and PurchaseTokenModel = {
    CCTokens               : decimal
    BuyTokens              : decimal
    CCAddress              : string
    TotalPrice             : decimal
    IsLoading              : bool
}

and VerificationModel = {
    CurrentTab             : int
} 
 
and InvestmentModel = {
    Coinbase               : string
    Transactions           : Web3Types.Transaction list
    IsLoading              : bool
} 

type TostrStatus = Success | Warning | Err | Info

let toastrCommon (status:TostrStatus) text =   
                    Toastr.message text
                    |> Toastr.withProgressBar
                    |> Toastr.position BottomRight
                    |> Toastr.timeout 2000
                    |> match status with
                        | TostrStatus.Success -> Toastr.success
                        | TostrStatus.Err -> Toastr.error
                        | TostrStatus.Warning -> Toastr.warning
                        | TostrStatus.Info -> Toastr.info
let toastrSuccess text = toastrCommon TostrStatus.Success text
let toastrError text = toastrCommon TostrStatus.Err text
let toastrWarning text = toastrCommon TostrStatus.Warning text
let toastrInfo text = toastrCommon TostrStatus.Info text
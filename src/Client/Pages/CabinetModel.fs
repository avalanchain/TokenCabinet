module Client.CabinetModel 

open Shared
open LoginCommon
open Shared.WalletPublic
open Elmish.Toastr


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
    | BuyTokens           //of decimal
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
    IsConnecting           : bool
} 

and VerifiacationModel = {
    CurrentTab             : int
} 


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
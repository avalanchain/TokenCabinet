module Client.CabinetModel 

open Shared
open LoginCommon
open Shared.WalletPublic
open Elmish.Toastr
open Fable.PowerPack
open System.Transactions
open Web3Types
open System.Transactions

type Model = {
    Auth                   : AuthModel
    // Loading     : bool  
    CryptoCurrencies       : ViewModels.CryptoCurrency list
    CurrenciesCurentPrices : ViewModels.CurrencyPriceTick 
    ActiveSymbol           : CryptoCurrencySymbol

    TokenSale              : ViewModels.TokenSale option
    FullCustomer           : ViewModels.FullCustomer option
    Transactions           : ViewModels.ETransaction list option

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
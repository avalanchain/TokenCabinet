module Client.CabinetModel 

open Shared
open LoginCommon

type Model = {
    Auth                   : AuthModel
    // Loading     : bool  
    CryptoCurrencies       : ViewModels.CryptoCurrency list
    CurrenciesCurentPrices : ViewModels.CurrencyPriceTick

    TokenSale              : ViewModels.TokenSale option
    FullCustomer           : ViewModels.FullCustomer option
}
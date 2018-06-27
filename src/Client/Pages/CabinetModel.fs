module Client.CabinetModel 

open Shared

type Model = {
    // Auth        : AuthModel option
    // Loading     : bool  
    CryptoCurrencies      : CryptoCurrencies.CryptoCurrency list
    CurrenciesCurentPrices: ViewModels.CurrencyPriceTick

    TokenSale             : ViewModels.TokenSale option
    }
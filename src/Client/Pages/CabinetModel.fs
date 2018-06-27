module Client.CabinetModel 

open Shared

type Model = {
    // Auth        : AuthModel option
    // Loading     : bool  
    CryptoCurrencies      : ViewModels.CryptoCurrency list
    CurrenciesCurentPrices: ViewModels.CurrencyPriceTick

    TokenSale             : ViewModels.TokenSale option
    FullCustomer          : ViewModels.FullCustomer option
    }
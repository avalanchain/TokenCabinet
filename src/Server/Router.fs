module Router

open Saturn
open Giraffe.Core
open Giraffe.ResponseWriters


let browser = pipeline {
    plug acceptHtml
    plug putSecureBrowserHeaders
    plug fetchSession
    set_header "x-pipeline-type" "Browser"
}

let defaultView = scope {
    get "/" (htmlView Index.layout)
    get "/index.html" (redirectTo false "/")
    get "/default.html" (redirectTo false "/")
}

let browserRouter = scope {
    not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
    pipe_through browser //Use the default browser pipeline

    forward "" defaultView //Use the default view

    forward "/verifications" Verifications.Controller.resource
    forward "/cryptocurrencies" CryptoCurrencies.Controller.resource
    forward "/saletokens" SaleTokens.Controller.resource
    forward "/tokensaledeals" TokenSaleDeals.Controller.resource
    forward "/tokensales" TokenSales.Controller.resource
    forward "/tokensalestages" TokenSaleStages.Controller.resource
    forward "/tokensalestagestatusids" TokenSaleStageStatusIds.Controller.resource
    forward "/tokensalestagestatuses" TokenSaleStageStatuses.Controller.resource
    forward "/tokensalestatusids" TokenSaleStatusIds.Controller.resource
    forward "/tokensalestatuses" TokenSaleStatuses.Controller.resource
    forward "/cryptocurrencyprices" CryptoCurrencyPrices.Controller.resource
    forward "/customers" Customers.Controller.resource
    forward "/customerpreferences" CustomerPreferences.Controller.resource
    forward "/customerverificationevents" CustomerVerificationEvents.Controller.resource
    forward "/tokensalestagestatusids" TokenSaleStageStatusIds.Controller.resource
    forward "/investmenttranstatusids" InvestmentTranStatusIds.Controller.resource
    forward "/investments" Investments.Controller.resource
    forward "/tokensalestatusids" TokenSaleStatusIds.Controller.resource
}

//Other scopes may use different pipelines and error handlers

// let api = pipeline {
//     plug acceptJson
//     set_header "x-pipeline-type" "Api"
// }

// let apiRouter = scope {
//     error_handler (text "Api 404")
//     pipe_through api
//
//     forward "/someApi" someScopeOrController
// }

let router = scope {
    // forward "/api" apiRouter
    forward "" browserRouter
}
module Seed

open Shared.ViewModels
open System
open Dapper

open FSharp.Control.Tasks.V2.ContextInsensitive

let createCustomer email password ethAddress : Customers.Customer =
    let id = System.Guid.NewGuid().ToString("N") 
    {   Id              = id
        Email           = email
        FirstName       = ""
        LastName        = ""
        EthAddress      = ethAddress
        Password        = password
        PasswordSalt    = id + email
        Avatar          = "MyPicture"
        CustomerTier    = Tier1.ToString() }


let seedT connectionString deleteAll insert lst = task {
    let! _ = deleteAll connectionString
    for l in lst do
        printfn "Seeding Type: %A" (l.GetType())
        let! res = insert connectionString l
        printfn "Seeding Result: %A" res
}

let saleTokenSeed connectionString = 
    let lst: SaleTokens.SaleToken list = 
        [{  Id = "AIM"
            Name = "AIM Network"
            LogoUrl = "assets/AIMLogo.jpg"
            TotalSupply = 100_000_000M }] 
    lst |> seedT connectionString SaleTokens.Database.deleteAll SaleTokens.Database.insert 

let stageStatusesSeed connectionString = 
    let lst: TokenSaleStageStatuses.TokenSaleStageStatus list = 
        [   {   Id                  = 1
                TokenSaleStageId    = 1
                Status              = TokenSaleStageStatus.Completed.ToString()
                CreatedOn           = DateTime.UtcNow
                CreatedBy           = "Initial"
                Proof               = "Initial Proof" }
            {   Id                  = 2
                TokenSaleStageId    = 2
                Status              = TokenSaleStageStatus.Active.ToString()
                CreatedOn           = DateTime.UtcNow
                CreatedBy           = "Initial"
                Proof               = "Initial Proof" }
            {   Id                  = 3
                TokenSaleStageId    = 3
                Status              = TokenSaleStageStatus.Expectation.ToString()
                CreatedOn           = DateTime.UtcNow
                CreatedBy           = "Initial"
                Proof               = "Initial Proof" }]
    lst |> seedT connectionString TokenSaleStageStatuses.Database.deleteAll TokenSaleStageStatuses.Database.insert  
let stagesSeed connectionString = 
    let lst: TokenSaleStages.TokenSaleStage list = 
        [   {   Id                  = 1
                TokenSaleId         = 1
                Name                = "Private Sale"
                CapEth              = 300M
                CapUsd              = 150_000M
                StartDate           = DateTime.Today
                EndDate             = DateTime.Today.AddMonths 1 
                CreatedOn           = DateTime.UtcNow
                CreatedBy           = "Initial"
                Proof               = "Initial Proof" }
            {   Id                  = 2
                TokenSaleId         = 1
                Name                = "Pre ICO"
                CapEth              = 1000M
                CapUsd              = 500_000M
                StartDate           = DateTime.Today.AddMonths 1
                EndDate             = DateTime.Today.AddMonths 2 
                CreatedOn           = DateTime.UtcNow
                CreatedBy           = "Initial"
                Proof               = "Initial Proof" }
            {   Id                  = 3
                TokenSaleId         = 1
                Name                = "ICO"
                CapEth              = 30000M
                CapUsd              = 15_000_000M
                StartDate           = DateTime.Today.AddMonths 2
                EndDate             = DateTime.Today.AddMonths 3 
                CreatedOn           = DateTime.UtcNow
                CreatedBy           = "Initial"
                Proof               = "Initial Proof" } ]
    lst |> seedT connectionString TokenSaleStages.Database.deleteAll TokenSaleStages.Database.insert

let tokenSaleStatusSeed connectionString startDate endDate = 
    let lst: TokenSaleStatuses.TokenSaleStatus list = 
        [   {   Id                  = 1
                TokenSaleId         = 1
                TokenSaleStatus     = TokenSaleStatus.Active.ToString()
                ActiveStageId       = 2
                SaleTokenId         = 1
                PriceUsd            = 5M
                PriceEth            = 0.01M
                BonusPercent        = 20M
                BonusTokens         = 100M
                StartDate           = startDate
                EndDate             = endDate 
                CreatedOn           = DateTime.UtcNow
                CreatedBy           = "Initial"
                Proof               = "Initial Proof" } ]
    lst |> seedT connectionString TokenSaleStatuses.Database.deleteAll TokenSaleStatuses.Database.insert

let tokenSaleSeed connectionString startDate endDate = 
    let lst: TokenSales.TokenSale list = 
        [   {   Id          = 1
                Symbol      = "AIM"
                SoftCapEth  = 10_000_000M
                HardCapEth  = 50_000_000M
                SoftCapUsd  = 10_000_000M
                HardCapUsd  = 50_000_000M
                Expectations = 50_000_000M
                StartDate   = startDate
                EndDate     = endDate
                CreatedOn   = DateTime.UtcNow
                CreatedBy   = "Initial"
                Proof       = "Initial Proof" } ]
    lst |> seedT connectionString TokenSales.Database.deleteAll TokenSales.Database.insert

let customerPreferencesSeed connectionString =
    let lst: CustomerPreferences.CustomerPreference list = 
        [   {   Id          = Guid.NewGuid().ToString("N")
                Language    = CustomerPreferences.Validation.supportedLangs.[0] } ]
    lst |> seedT connectionString CustomerPreferences.Database.deleteAll CustomerPreferences.Database.insert       

let customerSeed connectionString = task {
    let! _ = WalletsKV.Database.deleteAll connectionString
    let lst: Customers.Customer list = 
        [ createCustomer "trader@cryptoinvestor.com" "!!!ChangeMe111" "0x001002003004005006007008009" ]
    return! lst |> seedT connectionString Customers.Database.deleteAll Customers.Database.insert  
}

let walletsSeed connectionString = task {
    let! _ = WalletsKV.Database.deleteAll connectionString
    ()
}

let authTokenSeed connectionString = task {
    let! _ = AuthTokens.Database.deleteAll connectionString
    ()
}


let seedAll connectionString = task {
    do! saleTokenSeed connectionString
    printfn "Seeding ..."
    do! stageStatusesSeed connectionString
    do! stagesSeed connectionString
    let startDate = DateTime.Today
    let endDate   = DateTime.Today.AddMonths 3
    do! tokenSaleStatusSeed connectionString startDate endDate
    do! tokenSaleSeed connectionString startDate endDate

    do! customerPreferencesSeed connectionString
    do! customerSeed connectionString

    do! walletsSeed connectionString
    do! authTokenSeed connectionString
}

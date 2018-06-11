namespace Migrations
open SimpleMigrations

[<Migration(201806101831L, "Create CryptoCurrencyPrices")>]
type CreateCryptoCurrencyPrices() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE CryptoCurrencyPrices(
      Id INT NOT NULL,
      CryptoCurrencyName TEXT NOT NULL,
      PriceUsd DECIMAL NOT NULL,
      PriceEth DECIMAL NOT NULL,
      PriceAt DATETIME NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE CryptoCurrencyPrices")

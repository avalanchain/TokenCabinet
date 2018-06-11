namespace Migrations
open SimpleMigrations

[<Migration(201806101821L, "Create TokenSaleDeals")>]
type CreateTokenSaleDeals() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE TokenSaleDeals(
      Id INT NOT NULL,
      SaleTokenId TEXT NOT NULL,
      PriceUsd DECIMAL NOT NULL,
      PriceEth DECIMAL NOT NULL,
      BonusPercent DECIMAL NOT NULL,
      BonusTokens DECIMAL NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE TokenSaleDeals")

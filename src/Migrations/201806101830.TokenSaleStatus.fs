namespace Migrations
open SimpleMigrations

[<Migration(201806101830L, "Create TokenSaleStatuses")>]
type CreateTokenSaleStatuses() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE TokenSaleStatuses(
      Id INT NOT NULL,
      TokenSaleId INT NOT NULL,
      TokenSaleStatusId INT NOT NULL,
      ActiveStageId INT NOT NULL,
      SaleTokenId INT NOT NULL,
      PriceUsd DECIMAL NOT NULL,
      PriceEth DECIMAL NOT NULL,
      BonusPercent DECIMAL NOT NULL,
      BonusTokens DECIMAL NOT NULL,
      StartDate DATETIME NOT NULL,
      EndDate DATETIME NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE TokenSaleStatuses")

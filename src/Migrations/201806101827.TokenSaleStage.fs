namespace Migrations
open SimpleMigrations

[<Migration(201806101827L, "Create TokenSaleStages")>]
type CreateTokenSaleStages() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE TokenSaleStages(
      Id INT NOT NULL,
      TokenSaleId INT NOT NULL,
      CapEth DECIMAL NOT NULL,
      CapUsd DECIMAL NOT NULL,
      StartDate DATETIME NOT NULL,
      EndDate DATETIME NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE TokenSaleStages")

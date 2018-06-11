namespace Migrations
open SimpleMigrations

[<Migration(201806101822L, "Create TokenSales")>]
type CreateTokenSales() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE TokenSales(
      Id INT NOT NULL,
      SaleTokenId TEXT NOT NULL,
      SoftCapEth DECIMAL NOT NULL,
      HardCapEth DECIMAL NOT NULL,
      SoftCapUsd DECIMAL NOT NULL,
      HardCapUsd DECIMAL NOT NULL,
      Expectations DECIMAL NOT NULL,
      StartDate DATETIME NOT NULL,
      EndDate DATETIME NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE TokenSales")

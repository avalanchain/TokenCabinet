namespace Migrations
open SimpleMigrations

[<Migration(201806101820L, "Create SaleTokens")>]
type CreateSaleTokens() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE SaleTokens(
      Id TEXT NOT NULL,
      Name TEXT NOT NULL,
      LogoUrl TEXT NOT NULL,
      UpdateUrl TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE SaleTokens")

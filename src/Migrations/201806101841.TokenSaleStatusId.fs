namespace Migrations
open SimpleMigrations

[<Migration(201806101841L, "Create TokenSaleStatusIds")>]
type CreateTokenSaleStatusIds() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE TokenSaleStatusIds(
      Id INT NOT NULL,
      Status TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE TokenSaleStatusIds")

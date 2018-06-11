namespace Migrations
open SimpleMigrations

[<Migration(201806101923L, "Create TokenSaleStageStatusIds")>]
type CreateTokenSaleStageStatusIds() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE TokenSaleStageStatusIds(
      Id INT NOT NULL,
      Status TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE TokenSaleStageStatusIds")

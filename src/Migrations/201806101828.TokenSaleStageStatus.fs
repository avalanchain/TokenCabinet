namespace Migrations
open SimpleMigrations

[<Migration(201806101828L, "Create TokenSaleStageStatuses")>]
type CreateTokenSaleStageStatuses() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE TokenSaleStageStatuses(
      Id INT NOT NULL,
      TokenSaleStageId INT NOT NULL,
      Status INT NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE TokenSaleStageStatuses")

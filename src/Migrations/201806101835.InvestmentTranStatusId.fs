namespace Migrations
open SimpleMigrations

[<Migration(201806101835L, "Create InvestmentTranStatusIds")>]
type CreateInvestmentTranStatusIds() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE InvestmentTranStatusIds(
      Id INT NOT NULL,
      Status TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE InvestmentTranStatusIds")

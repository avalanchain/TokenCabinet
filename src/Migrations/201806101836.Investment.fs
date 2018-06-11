namespace Migrations
open SimpleMigrations

[<Migration(201806101836L, "Create Investments")>]
type CreateInvestments() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Investments(
      Id TEXT NOT NULL,
      CustomerId TEXT NOT NULL,
      EventType TEXT NOT NULL,
      TokenSaleDealId INT NOT NULL,
      Date DATETIME NOT NULL,
      AmountEth DECIMAL NOT NULL,
      AmountTokens DECIMAL NOT NULL,
      Rate DECIMAL NOT NULL,
      InvestmentTranStatusId INT NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Investments")

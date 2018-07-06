namespace Migrations
open SimpleMigrations

[<Migration(201807061726L, "Create Wallets")>]
type CreateWallets() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Wallets(
      Id TEXT NOT NULL,
      CustomerId TEXT NOT NULL,
      Symbol TEXT NOT NULL,
      AccType TEXT NOT NULL,
      Address TEXT NOT NULL,
      PubKey TEXT NOT NULL,
      PrivKey TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Wallets")

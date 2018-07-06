namespace Migrations
open SimpleMigrations

[<Migration(201807061841L, "Create WalletsKV")>]
type CreateWalletsKV() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE WalletsKV(
      CustomerId TEXT NOT NULL,
      Wallet TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE WalletsKV")

namespace Migrations
open SimpleMigrations

[<Migration(201806092130L, "Create CryptoCurrencies")>]
type CreateCryptoCurrencies() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE CryptoCurrencies(
      Id TEXT NOT NULL,
      Name TEXT NOT NULL,
      LogoUrl TEXT NOT NULL,
      UpdateUrl TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE CryptoCurrencies")

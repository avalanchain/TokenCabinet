namespace Migrations
open SimpleMigrations

[<Migration(201808071352L, "Create PwdResetTokenInfos")>]
type CreatePwdResetTokenInfos() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE PwdResetTokenInfos(
      PwdResetToken TEXT NOT NULL,
      CustomerId TEXT NOT NULL,
      Issued DATETIME NOT NULL,
      Expires DATETIME NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE PwdResetTokenInfos")

namespace Migrations
open SimpleMigrations

[<Migration(201806092051L, "Create Verifications")>]
type CreateVerifications() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Verifications(
      Id TEXT NOT NULL,
      FirstName TEXT NOT NULL,
      LastName TEXT NOT NULL,
      MiddleName TEXT NOT NULL,
      Gender TEXT NOT NULL,
      DoB DATETIME NOT NULL,
      PassportNo TEXT NOT NULL,
      PassportCountry TEXT NOT NULL,
      RegistrationDate DATETIME NOT NULL,
      RegCountry TEXT NOT NULL,
      Address TEXT NOT NULL,
      City TEXT NOT NULL,
      PostCode TEXT NOT NULL,
      DocType TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Verifications")

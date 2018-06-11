namespace Migrations
open SimpleMigrations

[<Migration(201806101834L, "Create CustomerVerificationEvents")>]
type CreateCustomerVerificationEvents() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE CustomerVerificationEvents(
      Id TEXT NOT NULL,
      CustomerId TEXT NOT NULL,
      EventType TEXT NOT NULL,
      CreatedOn DATETIME NOT NULL,
      CreatedBy DATETIME NOT NULL,
      Proof TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE CustomerVerificationEvents")

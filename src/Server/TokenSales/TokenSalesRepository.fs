namespace TokenSales

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<TokenSale seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, SaleTokenId, SoftCapEth, HardCapEth, SoftCapUsd, HardCapUsd, Expectations, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSales" None

  let getById connectionString id : Task<Result<TokenSale option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, SaleTokenId, SoftCapEth, HardCapEth, SoftCapUsd, HardCapUsd, Expectations, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSales WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSales SET Id = @Id, SaleTokenId = @SaleTokenId, SoftCapEth = @SoftCapEth, HardCapEth = @HardCapEth, SoftCapUsd = @SoftCapUsd, HardCapUsd = @HardCapUsd, Expectations = @Expectations, StartDate = @StartDate, EndDate = @EndDate, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSales(Id, SaleTokenId, SoftCapEth, HardCapEth, SoftCapUsd, HardCapUsd, Expectations, StartDate, EndDate, CreatedOn, CreatedBy, Proof) VALUES (@Id, @SaleTokenId, @SoftCapEth, @HardCapEth, @SoftCapUsd, @HardCapUsd, @Expectations, @StartDate, @EndDate, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSales WHERE Id=@Id" (dict ["id" => id])


namespace TokenSaleStages

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<TokenSaleStage seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, TokenSaleId, CapEth, CapUsd, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSaleStages" None

  let getById connectionString id : Task<Result<TokenSaleStage option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, TokenSaleId, CapEth, CapUsd, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSaleStages WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSaleStages SET Id = @Id, TokenSaleId = @TokenSaleId, CapEth = @CapEth, CapUsd = @CapUsd, StartDate = @StartDate, EndDate = @EndDate, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSaleStages(Id, TokenSaleId, CapEth, CapUsd, StartDate, EndDate, CreatedOn, CreatedBy, Proof) VALUES (@Id, @TokenSaleId, @CapEth, @CapUsd, @StartDate, @EndDate, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStages WHERE Id=@Id" (dict ["id" => id])


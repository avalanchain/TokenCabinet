namespace TokenSaleStageStatuses

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<TokenSaleStageStatus seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, TokenSaleStageId, Status, CreatedOn, CreatedBy, Proof FROM TokenSaleStageStatuses" None

  let getById connectionString id : Task<Result<TokenSaleStageStatus option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, TokenSaleStageId, Status, CreatedOn, CreatedBy, Proof FROM TokenSaleStageStatuses WHERE Id=@Id LIMIT 1" (Some <| dict ["Id" => id])

  let getByStageId connectionString stageId : Task<Result<TokenSaleStageStatus option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, TokenSaleStageId, Status, CreatedOn, CreatedBy, Proof FROM TokenSaleStageStatuses WHERE TokenSaleStageId=@stageId ORDER BY CreatedOn DESC LIMIT 1" (Some <| dict ["stageId" => stageId])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSaleStageStatuses SET Id = @Id, TokenSaleStageId = @TokenSaleStageId, Status = @Status, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSaleStageStatuses(Id, TokenSaleStageId, Status, CreatedOn, CreatedBy, Proof) VALUES (@Id, @TokenSaleStageId, @Status, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStageStatuses WHERE Id=@Id" (dict ["id" => id])

  let deleteAll connectionString : Task<Result<int, exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStageStatuses" None
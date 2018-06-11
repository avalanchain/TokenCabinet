namespace TokenSaleStageStatusIds

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<TokenSaleStageStatusId seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, Status FROM TokenSaleStageStatusIds" None

  let getById connectionString id : Task<Result<TokenSaleStageStatusId option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, Status FROM TokenSaleStageStatusIds WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSaleStageStatusIds SET Id = @Id, Status = @Status WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSaleStageStatusIds(Id, Status) VALUES (@Id, @Status)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStageStatusIds WHERE Id=@Id" (dict ["id" => id])


namespace TokenSaleStatusIds

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<TokenSaleStatusId seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, Status FROM TokenSaleStatusIds" None

  let getById connectionString id : Task<Result<TokenSaleStatusId option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, Status FROM TokenSaleStatusIds WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSaleStatusIds SET Id = @Id, Status = @Status WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSaleStatusIds(Id, Status) VALUES (@Id, @Status)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStatusIds WHERE Id=@Id" (dict ["id" => id])


namespace InvestmentTranStatusIds

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<InvestmentTranStatusId seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, Status FROM InvestmentTranStatusIds" None

  let getById connectionString id : Task<Result<InvestmentTranStatusId option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, Status FROM InvestmentTranStatusIds WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE InvestmentTranStatusIds SET Id = @Id, Status = @Status WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO InvestmentTranStatusIds(Id, Status) VALUES (@Id, @Status)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM InvestmentTranStatusIds WHERE Id=@Id" (dict ["id" => id])


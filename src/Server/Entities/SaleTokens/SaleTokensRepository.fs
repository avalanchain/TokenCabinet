namespace SaleTokens

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<SaleToken seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, Name, LogoUrl, UpdateUrl FROM SaleTokens" None

  let getById connectionString id : Task<Result<SaleToken option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, Name, LogoUrl, UpdateUrl FROM SaleTokens WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE SaleTokens SET Id = @Id, Name = @Name, LogoUrl = @LogoUrl, UpdateUrl = @UpdateUrl WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO SaleTokens(Id, Name, LogoUrl, UpdateUrl) VALUES (@Id, @Name, @LogoUrl, @UpdateUrl)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM SaleTokens WHERE Id=@Id" (dict ["id" => id])


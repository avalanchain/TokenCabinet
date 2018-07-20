namespace SaleTokens

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<SaleToken seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, Name, LogoUrl, TotalSupply FROM SaleTokens" None

  let getById connectionString id : Task<Result<SaleToken option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, Name, LogoUrl, TotalSupply FROM SaleTokens WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE SaleTokens SET Id = @Id, Name = @Name, LogoUrl = @LogoUrl, TotalSupply = @TotalSupply WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO SaleTokens(Id, Name, LogoUrl, TotalSupply) VALUES (@Id, @Name, @LogoUrl, @TotalSupply)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM SaleTokens WHERE Id=@Id" (dict ["id" => id])

  let deleteAll connectionString : Task<Result<int, exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM SaleTokens" None
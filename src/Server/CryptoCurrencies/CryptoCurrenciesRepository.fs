namespace CryptoCurrencies

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<CryptoCurrency seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, Name, LogoUrl, UpdateUrl FROM CryptoCurrencies" None

  let getById connectionString id : Task<Result<CryptoCurrency option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, Name, LogoUrl, UpdateUrl FROM CryptoCurrencies WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE CryptoCurrencies SET Id = @Id, Name = @Name, LogoUrl = @LogoUrl, UpdateUrl = @UpdateUrl WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO CryptoCurrencies(Id, Name, LogoUrl, UpdateUrl) VALUES (@Id, @Name, @LogoUrl, @UpdateUrl)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM CryptoCurrencies WHERE Id=@Id" (dict ["id" => id])


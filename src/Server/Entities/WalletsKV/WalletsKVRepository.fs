namespace WalletsKV

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<WalletKV seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT CustomerId, Wallet FROM WalletsKV" None

  let getById connectionString id : Task<Result<WalletKV option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT CustomerId, Wallet FROM WalletsKV WHERE CustomerId=@CustomerId" (Some <| dict ["CustomerId" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE WalletsKV SET CustomerId = @CustomerId, Wallet = @Wallet WHERE CustomerId=@CustomerId" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO WalletsKV(CustomerId, Wallet) VALUES (@CustomerId, @Wallet)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM WalletsKV WHERE CustomerId=@CustomerId" (dict ["id" => id])

  let deleteAll connectionString : Task<Result<int, exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM WalletsKV" None

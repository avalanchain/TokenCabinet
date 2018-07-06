namespace Wallets

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<WalletItem seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, CustomerId, Symbol, AccType, Address, PubKey, PrivKey FROM Wallets" None

  let getById connectionString id : Task<Result<WalletItem option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, CustomerId, Symbol, AccType, Address, PubKey, PrivKey FROM Wallets WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE Wallets SET Id = @Id, CustomerId = @CustomerId, Symbol = @Symbol, AccType = @AccType, Address = @Address, PubKey = @PubKey, PrivKey = @PrivKey WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO Wallets(Id, CustomerId, Symbol, AccType, Address, PubKey, PrivKey) VALUES (@Id, @CustomerId, @Symbol, @AccType, @Address, @PubKey, @PrivKey)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM Wallets WHERE Id=@Id" (dict ["id" => id])

  let deleteAll connectionString : Task<Result<int, exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM Wallets" None
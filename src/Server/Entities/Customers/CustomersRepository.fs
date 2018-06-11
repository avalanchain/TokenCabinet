namespace Customers

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<Customer seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, FirstName, LastName, EthAddress, Password, PasswordSalt, Avatar FROM Customers" None

  let getById connectionString id : Task<Result<Customer option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, FirstName, LastName, EthAddress, Password, PasswordSalt, Avatar FROM Customers WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE Customers SET Id = @Id, FirstName = @FirstName, LastName = @LastName, EthAddress = @EthAddress, Password = @Password, PasswordSalt = @PasswordSalt, Avatar = @Avatar WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO Customers(Id, FirstName, LastName, EthAddress, Password, PasswordSalt, Avatar) VALUES (@Id, @FirstName, @LastName, @EthAddress, @Password, @PasswordSalt, @Avatar)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM Customers WHERE Id=@Id" (dict ["id" => id])


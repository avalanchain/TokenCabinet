namespace Verifications

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<Verification seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, FirstName, LastName, MiddleName, Gender, DoB, PassportNo, PassportCountry, RegistrationDate, RegCountry, Address, City, PostCode, DocType FROM Verifications" None

  let getById connectionString id : Task<Result<Verification option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, FirstName, LastName, MiddleName, Gender, DoB, PassportNo, PassportCountry, RegistrationDate, RegCountry, Address, City, PostCode, DocType FROM Verifications WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE Verifications SET Id = @Id, FirstName = @FirstName, LastName = @LastName, MiddleName = @MiddleName, Gender = @Gender, DoB = @DoB, PassportNo = @PassportNo, PassportCountry = @PassportCountry, RegistrationDate = @RegistrationDate, RegCountry = @RegCountry, Address = @Address, City = @City, PostCode = @PostCode, DocType = @DocType WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO Verifications(Id, FirstName, LastName, MiddleName, Gender, DoB, PassportNo, PassportCountry, RegistrationDate, RegCountry, Address, City, PostCode, DocType) VALUES (@Id, @FirstName, @LastName, @MiddleName, @Gender, @DoB, @PassportNo, @PassportCountry, @RegistrationDate, @RegCountry, @Address, @City, @PostCode, @DocType)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM Verifications WHERE Id=@Id" (dict ["id" => id])


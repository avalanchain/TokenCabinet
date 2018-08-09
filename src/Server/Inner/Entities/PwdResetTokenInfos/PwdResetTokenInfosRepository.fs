namespace PwdResetTokenInfos

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

open Shared.Auth

module Database =
  let getAll connectionString : Task<Result<PwdResetTokenInfo seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT PwdResetToken, CustomerId, Issued, Expires FROM PwdResetTokenInfos" None

  let getByPwdResetToken connectionString (PwdResetToken pwdResetToken) : Task<Result<PwdResetTokenInfo option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT PwdResetToken, CustomerId, Issued, Expires FROM PwdResetTokenInfos WHERE PwdResetToken=@PwdResetToken" (Some <| dict ["PwdResetToken" => pwdResetToken])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE PwdResetTokenInfos SET PwdResetToken = @PwdResetToken, CustomerId = @CustomerId, Issued = @Issued, Expires = @Expires WHERE PwdResetToken=@PwdResetToken" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO PwdResetTokenInfos(PwdResetToken, CustomerId, Issued, Expires) VALUES (@PwdResetToken, @CustomerId, @Issued, @Expires)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM PwdResetTokenInfos WHERE PwdResetToken=@PwdResetToken" (dict ["id" => id])


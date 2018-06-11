namespace Investments

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<Investment seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, CustomerId, EventType, TokenSaleDealId, Date, AmountEth, AmountTokens, Rate, InvestmentTranStatusId, CreatedOn, CreatedBy, Proof FROM Investments" None

  let getById connectionString id : Task<Result<Investment option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, CustomerId, EventType, TokenSaleDealId, Date, AmountEth, AmountTokens, Rate, InvestmentTranStatusId, CreatedOn, CreatedBy, Proof FROM Investments WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE Investments SET Id = @Id, CustomerId = @CustomerId, EventType = @EventType, TokenSaleDealId = @TokenSaleDealId, Date = @Date, AmountEth = @AmountEth, AmountTokens = @AmountTokens, Rate = @Rate, InvestmentTranStatusId = @InvestmentTranStatusId, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO Investments(Id, CustomerId, EventType, TokenSaleDealId, Date, AmountEth, AmountTokens, Rate, InvestmentTranStatusId, CreatedOn, CreatedBy, Proof) VALUES (@Id, @CustomerId, @EventType, @TokenSaleDealId, @Date, @AmountEth, @AmountTokens, @Rate, @InvestmentTranStatusId, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM Investments WHERE Id=@Id" (dict ["id" => id])


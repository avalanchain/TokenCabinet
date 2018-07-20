namespace TokenSaleStatuses

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<TokenSaleStatus seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, TokenSaleId, TokenSaleStatus, ActiveStageId, SaleTokenId, PriceUsd, PriceEth, BonusPercent, BonusTokens, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSaleStatuses" None

  let getById connectionString id : Task<Result<TokenSaleStatus option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, TokenSaleId, TokenSaleStatus, ActiveStageId, SaleTokenId, PriceUsd, PriceEth, BonusPercent, BonusTokens, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSaleStatuses WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSaleStatuses SET Id = @Id, TokenSaleId = @TokenSaleId, TokenSaleStatus = @TokenSaleStatus, ActiveStageId = @ActiveStageId, SaleTokenId = @SaleTokenId, PriceUsd = @PriceUsd, PriceEth = @PriceEth, BonusPercent = @BonusPercent, BonusTokens = @BonusTokens, StartDate = @StartDate, EndDate = @EndDate, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSaleStatuses(Id, TokenSaleId, TokenSaleStatus, ActiveStageId, SaleTokenId, PriceUsd, PriceEth, BonusPercent, BonusTokens, StartDate, EndDate, CreatedOn, CreatedBy, Proof) VALUES (@Id, @TokenSaleId, @TokenSaleStatus, @ActiveStageId, @SaleTokenId, @PriceUsd, @PriceEth, @BonusPercent, @BonusTokens, @StartDate, @EndDate, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStatuses WHERE Id=@Id" (dict ["id" => id])

  let deleteAll connectionString : Task<Result<int, exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStatuses" None
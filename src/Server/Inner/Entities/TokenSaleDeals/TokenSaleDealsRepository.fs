namespace TokenSaleDeals

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<TokenSaleDeal seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, SaleTokenId, PriceUsd, PriceEth, BonusPercent, BonusTokens, CreatedOn, CreatedBy, Proof FROM TokenSaleDeals" None

  let getById connectionString id : Task<Result<TokenSaleDeal option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, SaleTokenId, PriceUsd, PriceEth, BonusPercent, BonusTokens, CreatedOn, CreatedBy, Proof FROM TokenSaleDeals WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSaleDeals SET Id = @Id, SaleTokenId = @SaleTokenId, PriceUsd = @PriceUsd, PriceEth = @PriceEth, BonusPercent = @BonusPercent, BonusTokens = @BonusTokens, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSaleDeals(Id, SaleTokenId, PriceUsd, PriceEth, BonusPercent, BonusTokens, CreatedOn, CreatedBy, Proof) VALUES (@Id, @SaleTokenId, @PriceUsd, @PriceEth, @BonusPercent, @BonusTokens, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleDeals WHERE Id=@Id" (dict ["id" => id])


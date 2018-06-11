namespace CryptoCurrencyPrices

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks

module Database =
  let getAll connectionString : Task<Result<CryptoCurrencyPrice seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, CryptoCurrencyName, PriceUsd, PriceEth, PriceAt, CreatedOn, CreatedBy, Proof FROM CryptoCurrencyPrices" None

  let getById connectionString id : Task<Result<CryptoCurrencyPrice option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, CryptoCurrencyName, PriceUsd, PriceEth, PriceAt, CreatedOn, CreatedBy, Proof FROM CryptoCurrencyPrices WHERE Id=@Id" (Some <| dict ["id" => id])

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE CryptoCurrencyPrices SET Id = @Id, CryptoCurrencyName = @CryptoCurrencyName, PriceUsd = @PriceUsd, PriceEth = @PriceEth, PriceAt = @PriceAt, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO CryptoCurrencyPrices(Id, CryptoCurrencyName, PriceUsd, PriceEth, PriceAt, CreatedOn, CreatedBy, Proof) VALUES (@Id, @CryptoCurrencyName, @PriceUsd, @PriceEth, @PriceAt, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM CryptoCurrencyPrices WHERE Id=@Id" (dict ["id" => id])


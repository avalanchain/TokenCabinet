namespace TokenSaleStages

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks
open FSharp.Control.Tasks
open System

module Database =
  let getAll connectionString : Task<Result<TokenSaleStage seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "SELECT Id, TokenSaleId, Name, CapEth, CapUsd, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSaleStages ORDER BY Id" None

  let getById connectionString id : Task<Result<TokenSaleStage option, exn>> =
    use connection = new SqliteConnection(connectionString)
    querySingle connection "SELECT Id, TokenSaleId, Name, CapEth, CapUsd, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSaleStages WHERE Id=@Id" (Some <| dict ["id" => id])

  // let getByTokenSaleId connectionString id : Task<Result<(TokenSaleStage*string) option, exn>> = task {
  //   use connection = new SqliteConnection(connectionString)
  //   let! stageRes = 
  //     querySingle connection "SELECT Id, TokenSaleId, CapEth, CapUsd, StartDate, EndDate, CreatedOn, CreatedBy, Proof FROM TokenSaleStages WHERE Id=@Id" (Some <| dict ["id" => id])
  //   let! stageResRes = match stageRes with
  //                       | Ok (Some stage) ->
  //                         task {  let! stageStatus = TokenSaleStageStatuses.Database.getByStageId connectionString stage.Id
  //                                 return (stage, stageStatus) |> Some |> Ok }
  //                       | Ok None -> Error (Exception "No TokenSaleStage found") |> Task.FromResult
  //                       | Error e -> Error e  |> Task.FromResult
  //   return match stageResRes with 
  //           | Ok (Some (stage, Ok (Some status))) -> 
  // }

  let update connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "UPDATE TokenSaleStages SET Id = @Id, TokenSaleId = @TokenSaleId, Name = @Name, CapEth = @CapEth, CapUsd = @CapUsd, StartDate = @StartDate, EndDate = @EndDate, CreatedOn = @CreatedOn, CreatedBy = @CreatedBy, Proof = @Proof WHERE Id=@Id" v

  let insert connectionString v : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "INSERT INTO TokenSaleStages(Id, TokenSaleId, Name, CapEth, CapUsd, StartDate, EndDate, CreatedOn, CreatedBy, Proof) VALUES (@Id, @TokenSaleId, @Name, @CapEth, @CapUsd, @StartDate, @EndDate, @CreatedOn, @CreatedBy, @Proof)" v

  let delete connectionString id : Task<Result<int,exn>> =
    use connection = new SqliteConnection(connectionString)
    execute connection "DELETE FROM TokenSaleStages WHERE Id=@Id" (dict ["id" => id])

  let deleteAll connectionString : Task<Result<TokenSaleStage seq, exn>> =
    use connection = new SqliteConnection(connectionString)
    query connection "DELETE FROM TokenSaleStages" None
module CoreLedger.Database.Core

open Npgsql
open System.Data.Common
open System.Threading.Tasks
open CoreLedger.Database.Types

type Configuration =
    { user: string
      password: string
      host: string
      port: int
      database: string
      minimumConnections: string
      maximumConnections: string }

    member this.toConnectionString() =
        $"Host={this.host};Username={this.user};Password={this.password};Database={this.database};MinPoolSize={this.minimumConnections};MaxPoolSize={this.maximumConnections}"

    member this.createDataSource() =
        NpgsqlDataSourceBuilder(this.toConnectionString ()).Build()

type Database(cfg: Configuration) =

    let mutable DataSource = cfg.createDataSource ()
    let mutable Connection = None

    member this.OpenConnection() =
        async {
            let! connection =
                DataSource
                    .OpenConnectionAsync()
                    .AsTask()
                    .ContinueWith(fun (task: Task<NpgsqlConnection>) ->
                        if task.IsCompletedSuccessfully then
                            Some task.Result
                        else
                            None)

            Connection <- connection
            return Connection
        }

    member this.CloseConnection() =
        async {
            Connection <-
                match Connection with
                | Some conn ->
                    conn.CloseAsync() |> Async.AwaitTask |> ignore
                    None
                | None -> None
        }

    member this.Stop() = this.CloseConnection()


let rec private readAllRows<'a> (reader: DbDataReader) (mapper: Mapper<'a>) acc =
    async {
        let! hasMoreRows = reader.ReadAsync()

        if hasMoreRows then
            let record = (mapper reader)
            return! readAllRows reader mapper (record :: acc)
        else
            return List.rev acc
    }

let private readResults<'a> (reader: DbDataReader) (mapper: Mapper<'a>) =
    async { return! readAllRows reader mapper [] }

let ExecuteQuery<'a> (database: Database) (query: string) (parameters: NpgsqlParameter list) (mapper: Mapper<'a>) =
    async {
        let! connection = database.OpenConnection()

        return!
            match connection with
            | Some conn ->
                async {
                    use cmd = new NpgsqlCommand(query, conn)
                    for parameter in parameters do
                        cmd.Parameters.Add parameter |> ignore
                    use! reader = cmd.ExecuteReaderAsync()
                    return! readResults reader mapper
                }
            | None -> [] |> Async.result
    }

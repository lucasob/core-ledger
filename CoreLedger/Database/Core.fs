module CoreLedger.Database.Core

open Npgsql
open System.Data.Common
open CoreLedger.Database.Types

type ConnectionConfig =
    { user: string
      password: string
      host: string
      port: int
      database: string
      minimumConnections: string
      maximumConnections: string }

    member this.toConnectionString() =
        $"Host={this.host};Username={this.user};Password={this.password};Database={this.database};MinPoolSize={this.minimumConnections};MaxPoolSize={this.maximumConnections}"

    member this.getConnection() =
        new NpgsqlConnection(this.toConnectionString ())

    member this.openConnection() =
        let connection = this.getConnection ()

        connection
            .OpenAsync()
            .ContinueWith(fun task ->
                if task.IsCompletedSuccessfully then
                    Ok connection
                else
                    Error "Unable to open connection")
        |> Async.AwaitTask

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

let ExecuteQuery<'a> (connection: NpgsqlConnection) (query: string) (mapper: Mapper<'a>) =
    async {
        use cmd = new NpgsqlCommand(query, connection)
        use! reader = cmd.ExecuteReaderAsync()
        return! readResults reader mapper
    }

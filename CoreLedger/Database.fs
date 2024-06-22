module CoreLedger.Database

open System
open System.Data.Common
open Npgsql

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
                    connection
                else
                    failwith "Unable to open connection")


let getConnection (cfg: ConnectionConfig) =
    new NpgsqlConnection(cfg.toConnectionString ())

let openConnection (conn: NpgsqlConnection) =
    async {
        do! conn.OpenAsync() |> Async.AwaitTask
        return conn
    }

type DbTypeMapper<'a> = DbDataReader -> 'a

let rec readAllRows<'a> (reader: DbDataReader) (mapper: DbTypeMapper<'a>) acc =
    async {
        let! hasMoreRows = reader.ReadAsync()

        if hasMoreRows then
            let record = (mapper reader)
            return! readAllRows reader mapper (record :: acc)
        else
            return List.rev acc
    }

let readResults<'a> (reader: DbDataReader) (mapper: DbDataReader -> 'a) =
    async { return! readAllRows reader mapper [] }

let executeQuery<'a> (cfg: ConnectionConfig) (query: string) (mapper: DbDataReader -> 'a) =
    async {
        use! c = cfg.openConnection ()
        use cmd = new NpgsqlCommand(query, c)
        use! reader = cmd.ExecuteReaderAsync()
        return! readResults reader mapper
    }

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

let getConnection (cfg: ConnectionConfig) =
    new NpgsqlConnection(cfg.toConnectionString ())

let openConnection (conn: NpgsqlConnection) =
    async {
        do! conn.OpenAsync() |> Async.AwaitTask
        return conn
    }

let readResults<'a> (reader: DbDataReader) (mapper: DbDataReader -> 'a) =
    async {
        let rec readAllRows acc =
            async {
                let! hasMoreRows = reader.ReadAsync() |> Async.AwaitTask

                if hasMoreRows then
                    let row = (mapper reader)
                    return! readAllRows (row :: acc)
                else
                    return List.rev acc
            }

        return readAllRows [] |> Async.RunSynchronously
    }

let executeQuery<'a> (conn: NpgsqlConnection) (query: string) (mapper: DbDataReader -> 'a) =
    async {
        use! c = openConnection conn
        use cmd = new NpgsqlCommand(query, c)
        use! reader = cmd.ExecuteReaderAsync() |> Async.AwaitTask
        return! readResults<'a> reader mapper
    }

module Demo =
    type Demo = { id: Guid; name: string }

    let into (reader: DbDataReader) : Demo =
        { id = reader.GetValue(0) |> string |> Guid
          name = reader.GetValue(1) |> string }

    let getAllDemos connection =
        executeQuery connection "select id, name from demo" into

module CoreLedger.Database.Component

open CoreLedger.Database.Core

type Database(cfg: ConnectionConfig) =

    let mutable Connection = cfg.getConnection ()

    member this.GetConnection() = Connection

    member this.Start() =
        async {
            let! connection = cfg.openConnection ()

            return
                match connection with
                | Ok conn ->
                    Connection <- conn
                    Ok this
                | Error e -> Error e
        }

    member this.Stop() =
        async {
            do! Connection.CloseAsync() |> Async.AwaitTask
            return Ok this
        }

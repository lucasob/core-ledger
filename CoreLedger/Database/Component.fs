module CoreLedger.Database.Component

open CoreLedger.Component.Core
open CoreLedger.Database.Core

type Database(cfg: ConnectionConfig) =

    let mutable Connection = cfg.getConnection ()

    member this.GetConnection() = Connection

    interface Component<Database> with
        member this.Start() =
            async {
                let! connection = cfg.openConnection ()

                return
                    match connection with
                    | Ok conn ->
                        Connection <- conn
                        Ok(this :> Component<Database>)
                    | Error e -> Error(e |> StartupError)
            }

        member this.Stop() =
            async {
                do! Connection.CloseAsync() |> Async.AwaitTask
                return Ok(this :> Component<Database>)
            }

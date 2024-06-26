module CoreLedger.Database.Component

open CoreLedger.Component.Core
open CoreLedger.Core

type Database(cfg: ConnectionConfig) =
    
    let mutable Connection = cfg.getConnection()
    
    member this.GetConnection () = Connection
    
    interface Component<Database> with
        member this.Start() =
            task {
                let c = cfg.openConnection()
                Connection <- c.Result
                return this
            }

        member this.Stop() =
            task {
                Connection.Close()
                return this
            }

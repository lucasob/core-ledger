module CoreLedgerTest.ComponentSupport

open CoreLedger.Component.Core
open CoreLedger.Database.Component

let StartDatabase config =
    async {
        let! startedDatabase = (Database(config) :> Component<Database>).Start()
        return match startedDatabase with
                    | Ok database -> database :?> Database
                    | Error (StartupError f) -> failwith f
    }
    
let StopDatabase (db: Database) =
    (db :> Component<Database>).Stop()

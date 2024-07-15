module CoreLedgerTest.ComponentSupport

open CoreLedger.Database.Component

let StartDatabase config =
    async {
        let! startedDatabase = Database(config).Start()
        return match startedDatabase with
                | Ok database -> database
                | Error f -> failwith f
    }
    
let StopDatabase (db: Database) = db.Stop()

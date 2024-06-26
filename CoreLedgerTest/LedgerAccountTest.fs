module CoreLedgerTest.LedgerAccountTest

open CoreLedger.Component.Core
open CoreLedger.Database.Component
open Xunit
open CoreLedger.Core
open CoreLedger.LedgerAccount

[<Fact>]
let ``Inserting a new Ledger Account returns a value, and can be retrieved again`` () =
    let cfg =
        { user = "admin"
          password = "password"
          host = "localhost"
          port = 5432
          database = "service"
          minimumConnections = "4"
          maximumConnections = "4" }
    
    let newLedgerAccount, retrievedLedgerAccount =
                async {
                    let db = Database(cfg)
                    (db :> Component<Database>).Start() |> ignore 
                    let! newLedgerAccount = insertAccount (db.GetConnection())
                    let! existingLedgerAccount = getAccountById (db.GetConnection()) newLedgerAccount.id
                    return newLedgerAccount, existingLedgerAccount.Value
                } |> Async.RunSynchronously

    Assert.Equal(newLedgerAccount, retrievedLedgerAccount)
    
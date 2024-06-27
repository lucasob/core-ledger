module CoreLedgerTest.LedgerAccountTest

open Xunit
open CoreLedger.Core
open CoreLedger.LedgerAccount
open CoreLedgerTest.ComponentSupport

[<Fact>]
let ``Inserting a new Ledger Account returns a value, and can be retrieved again`` () =
    
    let cfg = { user = "admin"
                password = "password"
                host = "localhost"
                port = 5432
                database = "service"
                minimumConnections = "4"
                maximumConnections = "4" }

    let newLedgerAccount, retrievedLedgerAccount =
                async {
                    let! db = StartDatabase cfg
                    let! newLedgerAccount = insertAccount (db.GetConnection())
                    let! existingLedgerAccount = getAccountById (db.GetConnection()) newLedgerAccount.id
                    let! _ = StopDatabase db
                    return newLedgerAccount, existingLedgerAccount.Value
                } |> Async.RunSynchronously

    Assert.Equal(newLedgerAccount, retrievedLedgerAccount)
    

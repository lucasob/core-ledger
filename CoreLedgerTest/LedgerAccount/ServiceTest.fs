module CoreLedgerTest.LedgerAccountTest

open Xunit
open CoreLedger.Database.Core
open CoreLedger.LedgerAccount.Service

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
                    let db = Database cfg
                    let ledgerAccountService = New db
                    
                    // Service under test
                    let! newLedgerAccount = ledgerAccountService.Insert ()
                    let! existingLedgerAccount = ledgerAccountService.GetById newLedgerAccount.id
                    
                    // End
                    let! _ = db.Stop()
                    return newLedgerAccount, existingLedgerAccount.Value
                } |> Async.RunSynchronously

    Assert.Equal(newLedgerAccount, retrievedLedgerAccount)
    
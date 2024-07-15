module CoreLedgerTest.LedgerAccountTest

open Xunit
open CoreLedger.Database.Core
open CoreLedger.LedgerAccount.Service
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
                    let service = New db
                    
                    // Service under test
                    let! newLedgerAccount = service.Insert ()
                    let! existingLedgerAccount = service.GetById newLedgerAccount.id
                    
                    // End
                    let! _ = StopDatabase db
                    return newLedgerAccount, existingLedgerAccount.Value
                } |> Async.RunSynchronously

    Assert.Equal(newLedgerAccount, retrievedLedgerAccount)
    
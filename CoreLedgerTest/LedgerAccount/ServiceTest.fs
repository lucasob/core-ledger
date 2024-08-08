module CoreLedgerTest.LedgerAccountTest

open Xunit
open CoreLedger.Database.Core
open CoreLedger.LedgerAccount

[<Fact>]
let ``Inserting a new Ledger Account returns a value, and can be retrieved again`` () =

    let newLedgerAccount, retrievedLedgerAccount =
                async {
                    let db = Database TestConfiguration.DatabaseConfiguration
                    let ledgerAccountService = Service.New db
                    
                    // Service under test
                    let! newLedgerAccount = ledgerAccountService.Insert ()
                    let! existingLedgerAccount = ledgerAccountService.GetById newLedgerAccount.id
                    
                    // End
                    let! _ = db.Stop()
                    return newLedgerAccount, existingLedgerAccount.Value
                } |> Async.RunSynchronously

    Assert.Equal(newLedgerAccount, retrievedLedgerAccount)
    
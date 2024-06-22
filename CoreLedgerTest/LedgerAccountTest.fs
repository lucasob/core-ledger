module CoreLedgerTest.LedgerAccountTest

open Xunit
open CoreLedger.Database
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
    
    let newLedgerAccount = insertAccount cfg |> Async.RunSynchronously
    
    let existingLedgerAccount = getAccountById cfg newLedgerAccount.id |> Async.RunSynchronously
    
    Assert.Equal (newLedgerAccount, existingLedgerAccount.Value)

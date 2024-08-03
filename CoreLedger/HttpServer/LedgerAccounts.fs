module CoreLedger.HttpServer.LedgerAccounts

open CoreLedger.LedgerAccount.Service
open FSharp.Json

let Create service =
    service.Insert () |> Async.RunSynchronously |> Json.serialize
    
module CoreLedgerTest.LedgerAccount.ApiTest

open Xunit
open System.Net
open System.Net.Http
open FSharp.Json
open Suave.Testing
open CoreLedger.Server


[<Fact>]
let ``POSTing to LedgerAccounts will create a new account`` () =
    let _ctx = runWith config webApp

    let requestBody =
        {| Hello = "World" |} |> Json.serialize |> System.Text.Encoding.UTF8.GetBytes

    let hostUri = config.bindings.Head
    let uri = $"http://localhost:{hostUri.socketBinding.port}/ledger-accounts"

    use client = new HttpClient()

    let res =
        client.PostAsync(uri, (new ByteArrayContent(requestBody)))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    Assert.Equal(HttpStatusCode.Created, res.StatusCode)

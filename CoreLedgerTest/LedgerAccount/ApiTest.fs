module CoreLedgerTest.LedgerAccount.ApiTest

open System
open System.IO
open CoreLedger.LedgerAccount.Core
open Xunit
open System.Net
open System.Net.Http
open FSharp.Json
open Suave.Testing
open CoreLedger.HttpServer


[<Fact>]
let ``POSTing to LedgerAccounts will create a new account`` () =
    let system = CoreLedger.System.New()
    let _ctx = runWith Server.Configuration (Server.webService system)

    let requestBody = "{}" |> System.Text.Encoding.UTF8.GetBytes

    let hostUri = Server.Configuration.bindings.Head
    let uri = $"http://localhost:{hostUri.socketBinding.port}/ledger-accounts"

    use client = new HttpClient()

    let res =
        client.PostAsync(uri, (new ByteArrayContent(requestBody)))
        |> Async.AwaitTask
        |> Async.RunSynchronously
        
    let stream = res.Content.ReadAsStream()
    let reader = new StreamReader(stream)
    let jsonString = reader.ReadToEnd()
    
    let createdAccount = Json.deserialize<LedgerAccount> jsonString

    Assert.Equal(HttpStatusCode.Created, res.StatusCode)
    Assert.Equal(createdAccount.balance, 0.00)

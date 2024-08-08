module CoreLedgerTest.LedgerAccount.ApiTest

open System.IO
open CoreLedger.LedgerAccount.Core
open CoreLedgerTest
open Suave
open Xunit
open System.Net
open System.Net.Http
open FSharp.Json
open Suave.Testing
open CoreLedger.HttpServer
open CoreLedger.System


[<Fact>]
let ``POSTing to LedgerAccounts will create a new account`` () =
    let system =
        CoreLedger.System.New({ DatabaseConfig = TestConfiguration.DatabaseConfiguration })
        
    let serverConfig = (TestConfiguration.ServerConfiguration ()) 

    let _ctx = runWith serverConfig (Server.webService system)

    let requestBody = "{}" |> System.Text.Encoding.UTF8.GetBytes

    let uri = $"http://localhost:{serverConfig.bindings.Head.socketBinding.port}/ledger-accounts"

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

[<Fact>]
let ``Consecutive, different requests create separate accounts`` () =
    let system =
        CoreLedger.System.New({ DatabaseConfig = TestConfiguration.DatabaseConfiguration })

    let serverConfig = (TestConfiguration.ServerConfiguration ()) 
    let _ctx = runWith serverConfig (Server.webService system)

    let requestBody = "{}" |> System.Text.Encoding.UTF8.GetBytes

    let uri = $"http://localhost:{serverConfig.bindings.Head.socketBinding.port}/ledger-accounts"

    use client = new HttpClient()

    let requests =
        [ (new ByteArrayContent(requestBody)); (new ByteArrayContent(requestBody)) ]

    let responses =
        requests
        |> List.map (fun req -> client.PostAsync(uri, req) |> Async.AwaitTask)
        |> Async.Parallel
        |> Async.map (
            Array.map (fun res ->
                {| Status = res.StatusCode
                   Body =
                    (new StreamReader(res.Content.ReadAsStream())).ReadToEnd()
                    |> Json.deserialize<LedgerAccount> |})
        )
        |> Async.RunSynchronously

    for response in responses do
        Assert.Equal(HttpStatusCode.Created, response.Status)
        Assert.Equal(response.Body.balance, 0.00)

    // Of course what really matters is that we have 2, distinct LedgerAccounts
    Assert.NotEqual(responses.[0], responses.[1])

module Tests

open System
open FSharp.Json
open Xunit
open CoreLedger.Server
open Suave.Testing
open Suave

type test = { Hello: string }

[<Fact>]
let ``Spins up the API`` () =
    let ctx = runWith defaultConfig basemap
    let res = req HttpMethod.GET "/" None ctx

    let responseBody = Json.deserialize<test> res
    Expecto.Expect.equal responseBody { Hello = "World" }

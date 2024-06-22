module ServerTests

open System
open FSharp.Json
open Xunit
open CoreLedger.Server
open Suave.Testing
open Suave

type test = { Hello: string; Host: string }

[<Fact>]
let ``API can start, and responds on health endpoint`` () =
    let ctx = runWith defaultConfig webApp
    let res = req HttpMethod.GET "/health" None ctx

    let responseBody = Json.deserialize<test> res
    
    Expecto.Expect.equal responseBody.Hello "World" "Response body is correct"
    Expecto.Expect.isNotEmpty responseBody.Host "Some version is returned"

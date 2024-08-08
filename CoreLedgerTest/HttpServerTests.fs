module ServerTests

open CoreLedgerTest
open FSharp.Json
open Xunit
open CoreLedger.HttpServer
open CoreLedger.System
open Suave.Testing
open Suave

[<Fact>]
let ``API can start, and responds on health endpoint`` () =
    let system = CoreLedger.System.New({DatabaseConfig = TestConfiguration.DatabaseConfiguration})
    let ctx = runWith (TestConfiguration.ServerConfiguration ()) (Server.webService system)
    let res = req HttpMethod.GET "/health" None ctx

    let responseBody = Json.deserialize<Health.Health> res
    
    Expecto.Expect.isNotEmpty responseBody.Host "Some version is returned"

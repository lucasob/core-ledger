module ServerTests

open FSharp.Json
open Xunit
open CoreLedger.HttpServer
open Suave.Testing
open Suave

[<Fact>]
let ``API can start, and responds on health endpoint`` () =
    let system = CoreLedger.System.New()
    let ctx = runWith defaultConfig (Server.webService system)
    let res = req HttpMethod.GET "/health" None ctx

    let responseBody = Json.deserialize<Health.Health> res
    
    Expecto.Expect.isNotEmpty responseBody.Host "Some version is returned"

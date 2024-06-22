module CoreLedgerTest.QueryTest

open System.Collections
open Xunit
open CoreLedger.Database
open CoreLedger.Database.Demo

[<Fact>]
let ``This sucks bc it depends on DB state but hey is a starting point`` () =
    let cfg =
        { user = "admin"
          password = "password"
          host = "localhost"
          port = 5432
          database = "service"
          minimumConnections = "4"
          maximumConnections = "4" }

    let c = getConnection cfg
    let results = getAllDemos c |> Async.RunSynchronously

    let names = List.map (_.name) results
    let expected = [ "Lucas" ]

    Assert.Equal(names :> IEnumerable, expected :> IEnumerable)

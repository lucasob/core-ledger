module CoreLedgerTest.TestConfiguration

open System.Net
open Suave
open Suave.Sockets

let RandomPort () =
    (8080 + System.Random().Next(2000)) |> uint16

let ServerConfiguration () =
    { defaultConfig with
        bindings =
            [ { scheme = Protocol.HTTP
                socketBinding =
                  { ip = defaultConfig.bindings.Head.socketBinding.ip
                    port = RandomPort() } } ] }

let DatabaseConfiguration =
    { CoreLedger.System.DatabaseConfiguration with
        host = "localhost" }

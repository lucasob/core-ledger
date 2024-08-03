module CoreLedger.HttpServer.Server

open System.Threading
open FSharp.Json
open Suave
open CoreLedger.HttpServer
open Suave.Operators
open Suave.Sockets
open System.Net

let httpConfig =
    { scheme = Protocol.HTTP
      socketBinding =
        { ip = IPAddress.Parse "0.0.0.0"
          port = Port.Parse "8080" } }

let Configuration =
    { defaultConfig with
        bindings = [ httpConfig ] }

let webService _dependencies =
    choose [ Filters.path "/health" >=> choose [ Filters.GET >=> Successful.OK (Health.Get () |> Json.serialize) ]
             Filters.path "/ledger-accounts" >=> choose [ Filters.POST >=> Successful.CREATED (Health.Get () |> Json.serialize) ]
             RequestErrors.NOT_FOUND "BAD"]

type Server (configuration, app) =
    member this.Start(cancellationToken: CancellationToken) =
        let _, server = startWebServerAsync configuration app
        Async.Start(server, cancellationToken)

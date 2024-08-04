module CoreLedger.HttpServer.Server

open System.Threading
open FSharp.Json
open Suave
open Suave.Operators

open Suave.Sockets
open System.Net
open CoreLedger.HttpServer
open CoreLedger.System

let httpConfig =
    { scheme = Protocol.HTTP
      socketBinding =
        { ip = IPAddress.Parse "0.0.0.0"
          port = Port.Parse "8080" } }

let Configuration =
    { defaultConfig with
        bindings = [ httpConfig ] }

let webService dependencies =
    choose
        [ Filters.path "/health"
          >=> choose [ Filters.GET >=> Successful.OK(Health.Get() |> Json.serialize) ]
          Filters.path "/ledger-accounts"
          >=> choose
                  [ Filters.POST
                    >=> Writers.setHeader "Content-Type" "application/json"
                    >=> request (fun _r -> Successful.CREATED(LedgerAccounts.Create dependencies.LedgerAccountService)) ]
          RequestErrors.NOT_FOUND "BAD" ]

type Server(configuration, app) =
    member this.Start(cancellationToken: CancellationToken) =
        let _, server = startWebServerAsync configuration app
        Async.Start(server, cancellationToken)

﻿module CoreLedger.Main

open System
open System.Threading
open System.Runtime.Loader
open Suave
open Suave.Logging
open CoreLedger
open CoreLedger.HttpServer

type SignalSource =
    | TERM
    | INT

let handleShutdown (tokenSource: CancellationTokenSource) (logger: Logger) (source: SignalSource) =
    if not tokenSource.Token.IsCancellationRequested then
        logger.info (fun _ ->
            { name = [| "User" |]
              value = $"Shutdown initiated from {source}" |> Event
              fields = Map [ ("a", "b") ]
              timestamp = DateTime.timestamp DateTime.Now
              level = LogLevel.Info })

        tokenSource.Cancel()


[<EntryPoint>]
let main _ =
    let cancellationToken = new CancellationTokenSource()

    let serverConfig =
        { Server.Configuration with
            cancellationToken = cancellationToken.Token }

    // On ctrl-c (I don't think this runs in Docker)
    Console.CancelKeyPress.Add(fun _ -> handleShutdown cancellationToken serverConfig.logger SignalSource.INT)

    // On sig-term (passed in from another process, or parent)
    AssemblyLoadContext.Default.add_Unloading (fun _ ->
        handleShutdown cancellationToken serverConfig.logger SignalSource.TERM)

    // Create our system
    let system = System.New({ DatabaseConfig = System.DatabaseConfiguration })

    // Start the server asynchronously so we can control it
    let server = Server.Server(Server.Configuration, (Server.webService system))
    server.Start(cancellationToken.Token)

    // I am not sure if this is best practice in fsharp, but it seems to make sense
    let pos = WaitHandle.WaitAny([| cancellationToken.Token.WaitHandle |])
    printfn $"Cancellation received on ${pos}"

    0

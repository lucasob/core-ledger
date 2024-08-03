module CoreLedger.Server

open FSharp.Json
open Suave
open Suave.Operators
open System
open Suave.Sockets
open System.Net

let version () =
    let v = Environment.GetEnvironmentVariable "HOSTNAME"
    if String.IsNullOrEmpty v then "localhost" else v


let httpConfig =
    { scheme = Protocol.HTTP
      socketBinding =
        { ip = IPAddress.Parse "0.0.0.0"
          port = Port.Parse "8080" } }

let config =
    { defaultConfig with
        bindings = [ httpConfig ] }

let health = {| Hello = "World"; Host = version () |} |> Json.serialize

let webService _dependencies =
    choose [ Filters.path "/health" >=> choose [ Filters.GET >=> Successful.OK health ]
             Filters.path "/ledger-accounts" >=> choose [ Filters.POST >=> Successful.CREATED health ]
             RequestErrors.NOT_FOUND "BAD"]

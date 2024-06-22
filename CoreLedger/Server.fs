module CoreLedger.Server

open FSharp.Json
open Suave
open Suave.Operators
open System
open Suave.Sockets
open System.Net

let version () =
    let v = Environment.GetEnvironmentVariable "HOSTNAME"
    if String.IsNullOrEmpty v then
        "localhost"
    else
        v


let httpConfig =
    { scheme = Protocol.HTTP
      socketBinding =
        { ip = IPAddress.Parse "0.0.0.0"
          port = Port.Parse "8080" } }

let config = { defaultConfig with bindings = [ httpConfig ] }

let health = {| Hello = "World"; Host = version () |} |> Json.serialize

let webApp =
    choose [ Filters.GET >=> choose [ Filters.path "/health" >=> Successful.OK health ] ]

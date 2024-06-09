module CoreLedger.Server

open FSharp.Json
open Suave
open System
open Suave.Sockets
open System.Net

let version () =
    Environment.GetEnvironmentVariable "HOSTNAME"

let s: SocketBinding =
    { ip = IPAddress.Parse "0.0.0.0"
      port = Port.Parse "8080" }

let h: HttpBinding =
    { scheme = Protocol.HTTP
      socketBinding = s }

let config: SuaveConfig = { defaultConfig with bindings = [ h ] }

let helloWorld = {| Hello = "World" |} |> Json.serialize

let basemap = (Successful.OK helloWorld)
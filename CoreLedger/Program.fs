module CoreLedger.Main

open Suave
open CoreLedger.Server

[<EntryPoint>]
let main _ = 
    startWebServer config webApp
    0

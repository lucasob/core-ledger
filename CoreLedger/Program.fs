open Suave
open CoreLedger.Server

[<EntryPoint>]
let main _ = 
    startWebServer config basemap
    0

module CoreLedger.HttpServer.Health

open System
open System.Runtime.Serialization

let private version () =
    let host = Environment.GetEnvironmentVariable "HOSTNAME"
    if String.IsNullOrEmpty host then "localhost" else host
    
[<DataContract>]
type Health =
    {
        [<field: DataMember(Name="Host")>]
        Host: string;
    }
    
let Get () = { Host = version () }

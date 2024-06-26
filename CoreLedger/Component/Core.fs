module CoreLedger.Component.Core
open System.Threading.Tasks

type StartupError =
    | StartupError of string
type ShutdownError =
    | ShutdownError of string

type Component<'a> =
    abstract member Start: unit -> Async<Result<Component<'a>, StartupError>>
    abstract member Stop: unit -> Async<Result<Component<'a>, ShutdownError>>
 
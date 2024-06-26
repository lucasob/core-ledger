module CoreLedger.Component.Core
open System.Threading.Tasks

type Component<'a> =
    abstract member Start: unit -> Task<Component<'a>>
    abstract member Stop: unit -> Task<Component<'a>>
 
module CoreLedger.LedgerAccount.Core

open System
open System.Data.Common

type LedgerAccount =
    { id: Guid }

let toLedgerAccount (reader: DbDataReader) =
    { id = reader.GetValue(0) |> string |> Guid }

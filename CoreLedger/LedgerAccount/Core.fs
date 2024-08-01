module CoreLedger.LedgerAccount.Core

open System
open System.Data.Common

type LedgerAccount =
    { id: Guid
      balance: double }

let ToLedgerAccount (reader: DbDataReader) =
    { id = reader.GetValue(0) |> string |> Guid
      balance = reader.GetValue(1) |> string |> double }

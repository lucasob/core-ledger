module CoreLedger.LedgerAccount

open System
open System.Data.Common
open CoreLedger.Database

type LedgerAccount =
    { id: Guid }


let toLedgerAccount (reader: DbDataReader) =
    { id = reader.GetValue(0) |> string |> Guid }

let insertAccount (connection: ConnectionConfig) =
    async {
        let! result = executeQuery
                        connection
                        "insert into ledger_accounts (id) values (DEFAULT) returning id"
                        toLedgerAccount
        return List.head result
    }
    
let getAccountById (connection: ConnectionConfig) (accountId: Guid) =
    async {
        let! result = executeQuery
                        connection
                        $"select id from ledger_accounts where id = '{accountId}'"
                        toLedgerAccount
        return List.first result
    }

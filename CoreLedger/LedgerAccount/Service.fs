module CoreLedger.LedgerAccount.Service

open System
open CoreLedger.LedgerAccount.Core
open CoreLedger.Database.Core
open Npgsql

let private insertAccount (db: Database) =
    async {
        let queryString =
            "insert into ledger_accounts (id) values (DEFAULT) returning id, balance"

        let! result = ExecuteQuery db queryString [] ToLedgerAccount
        return List.head result
    }

let private getAccountById (db: Database) (accountId: Guid) =
    async {
        let queryString = "select id, balance from ledger_accounts where id = @p1"
        let parameters = [ NpgsqlParameter("@p1", accountId) ]
        let! result = ExecuteQuery db queryString parameters ToLedgerAccount
        return List.first result
    }

type Service =
    { GetById: Guid -> Async<Option<LedgerAccount>>
      Insert: unit -> Async<LedgerAccount> }

let New (db: Database) =
    { GetById = getAccountById db
      Insert = fun () -> insertAccount db }

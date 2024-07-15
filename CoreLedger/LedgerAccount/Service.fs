module CoreLedger.LedgerAccount.Service

open CoreLedger.Database.Component
open CoreLedger.LedgerAccount.Core
open Npgsql
open System
open CoreLedger.Database.Core

let private insertAccount (connection: NpgsqlConnection) =
    async {
        let! result =
            ExecuteQuery connection "insert into ledger_accounts (id) values (DEFAULT) returning id, balance" toLedgerAccount

        return List.head result
    }

let private getAccountById (connection: NpgsqlConnection) (accountId: Guid) =
    async {
        let! result = ExecuteQuery connection $"select id, balance from ledger_accounts where id = '{accountId}'" toLedgerAccount
        return List.first result
    }

type Service =
    { GetById: Guid -> Async<Option<LedgerAccount>>
      Insert: unit -> Async<LedgerAccount> }

let New (db: Database) =
    let g = getAccountById (db.GetConnection())
    let i = fun () -> insertAccount (db.GetConnection())
    
    { GetById = g
      Insert = i }

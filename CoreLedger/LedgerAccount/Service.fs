module CoreLedger.LedgerAccount.Service

open System
open CoreLedger.LedgerAccount.Core
open CoreLedger.Database.Core

let private insertAccount (db: Database) =
    async {
        let! result =
            ExecuteQuery db "insert into ledger_accounts (id) values (DEFAULT) returning id, balance" ToLedgerAccount

        return List.head result
    }

let private getAccountById (db: Database) (accountId: Guid) =
    async {
        let! connection = db.OpenConnection ()
        let! result = ExecuteQuery db $"select id, balance from ledger_accounts where id = '{accountId}'" ToLedgerAccount
        return List.first result
    }

type Service =
    { GetById: Guid -> Async<Option<LedgerAccount>>
      Insert: unit -> Async<LedgerAccount> }

let New (db: Database) =
    { GetById = getAccountById db
      Insert = fun () -> insertAccount db }

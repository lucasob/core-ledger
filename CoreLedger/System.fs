module CoreLedger.System

open CoreLedger.Database.Core

let databaseConfiguration =
    { user = "admin"
      password = "password"
      host = "db"
      port = 5432
      database = "service"
      minimumConnections = "4"
      maximumConnections = "4" }

type System =
    { Database: Database
      LedgerAccountService: LedgerAccount.Service.Service }

let New () =
    let database = Database(databaseConfiguration)
    let ledgerAccountService = LedgerAccount.Service.New(database)

    { Database = database
      LedgerAccountService = ledgerAccountService }

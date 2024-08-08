module CoreLedger.System

open CoreLedger.Database.Core

let DatabaseConfiguration =
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

type SystemConfig =
    { DatabaseConfig: Database.Core.Configuration }

let New config =
    let database = Database(config.DatabaseConfig)
    let ledgerAccountService = LedgerAccount.Service.New(database)

    { Database = database
      LedgerAccountService = ledgerAccountService }

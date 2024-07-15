module CoreLedger.Database.Types

open System.Data.Common

type Mapper<'a> = DbDataReader -> 'a

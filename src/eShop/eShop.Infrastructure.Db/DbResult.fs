[<AutoOpen>]
module eShop.Infrastructure.Db.DbResult

open System
open eShop.Infrastructure.FSharp

type DbReadError =
    | InvalidRecord of string
    | MissingRecord of string

type DbError =
    | Exception of Exception
    | Read of DbReadError

type DbResult<'a> = AsyncResult<'a, DbError>


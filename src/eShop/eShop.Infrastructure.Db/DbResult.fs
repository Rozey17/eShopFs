namespace eShop.Infrastructure.Db

open eShop.Infrastructure.FSharp

type DbReadError =
    | InvalidRecord of string
    | MissingRecord of string

type DbError =
    | Exception of exn
    | Read of DbReadError

type DbResult<'a> = AsyncResult<'a, DbError>


module eShop.Infrastructure.Db

open System.Data
open Dapper

let queryAsync<'Result> (connection: IDbConnection) (sql: string) : Async<'Result seq> =
    connection.QueryAsync<'Result>(sql)
    |> Async.AwaitTask

let parameterizedQueryAsync<'Result> (connection: IDbConnection) (sql: string) (param: _) : Async<'Result seq> =
    connection.QueryAsync<'Result>(sql, param)
    |> Async.AwaitTask

let tryParameterizedQuerySingleAsync<'Result> (connection: IDbConnection) (sql: string) (param: _) : Async<'Result option> =
    async {
        let! result =
            connection.QuerySingleOrDefaultAsync<'Result>(sql, param)
            |> Async.AwaitTask

        if isNull (box result) then
            return None
        else
            return Some result
    }

let parameterizedQuerySingleAsync<'Result> (connection: IDbConnection) (sql: string) (param: _) : Async<'Result> =
    connection.QuerySingleAsync<'Result>(sql, param)
    |> Async.AwaitTask

let executeAsync (connection: IDbConnection) (sql: string) =
    connection.ExecuteAsync(sql)
    |> Async.AwaitTask
    |> Async.Ignore

let parameterizedExecuteAsync (connection: IDbConnection) (sql: string) (param: _) =
    connection.ExecuteAsync(sql, param)
    |> Async.AwaitTask
    |> Async.Ignore

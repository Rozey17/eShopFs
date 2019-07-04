module eShop.Infrastructure.Db

open System.Collections.Generic
open System.Data
open System.Dynamic
open Dapper

let private mapToExpando (map: Map<string, string>) =
    let expando = ExpandoObject()
    let expandoDictionary = expando :> IDictionary<string, obj>
    for paramValue in map do
        expandoDictionary.Add(paramValue.Key, paramValue.Value :> obj)
    expando

let queryAsync<'Result> (connection: IDbConnection) (sql: string) : Async<'Result seq> =
    connection.QueryAsync<'Result>(sql)
    |> Async.AwaitTask

let parameterizedQueryAsync<'Result> (connection: IDbConnection) (sql: string) (param: _) : Async<'Result seq> =
    connection.QueryAsync<'Result>(sql, param)
    |> Async.AwaitTask

let mapParameterizedQueryAsync<'Result> connection sql param : Async<'Result seq> =
    let expando = mapToExpando param
    parameterizedQueryAsync connection sql expando

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

let mapParameterizedQuerySingleAsync<'Result> connection sql param: Async<'Result> =
    let expando = mapToExpando param
    parameterizedQuerySingleAsync connection sql expando

let executeAsync (connection: IDbConnection) (sql: string) =
    connection.ExecuteAsync(sql)
    |> Async.AwaitTask
    |> Async.Ignore

let parameterizedExecuteAsync (connection: IDbConnection) (sql: string) (param: _) =
    connection.ExecuteAsync(sql, param)
    |> Async.AwaitTask
    |> Async.Ignore

let mapParameterizedExecuteAsync connection sql param =
    let expando = mapToExpando param
    parameterizedExecuteAsync connection sql expando

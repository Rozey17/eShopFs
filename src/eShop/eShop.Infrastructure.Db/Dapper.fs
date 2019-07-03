module eShop.Infrastructure.Db.Dapper

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

let parametrizedQueryAsync<'Result> (connection: IDbConnection) (sql: string) (param: obj) : Async<'Result seq> =
    connection.QueryAsync<'Result>(sql, param)
    |> Async.AwaitTask

let mapParametrizedQueryAsync<'Result> connection sql param : Async<'Result seq> =
    let expando = mapToExpando param
    parametrizedQueryAsync connection sql expando

let tryParametrizedQuerySingleAsync<'Result> (connection: IDbConnection) (sql: string) (param: obj) : Async<'Result option> =
    async {
        let! result =
            connection.QuerySingleOrDefaultAsync<'Result>(sql, param)
            |> Async.AwaitTask

        if isNull (box result) then
            return None
        else
            return Some result
    }

let parametrizedQuerySingleAsync<'Result> (connection: IDbConnection) (sql: string) (param: obj) : Async<'Result> =
    connection.QuerySingleAsync<'Result>(sql, param)
    |> Async.AwaitTask

let mapParametrizedQuerySingleAsync<'Result> connection sql param: Async<'Result> =
    let expando = mapToExpando param
    parametrizedQuerySingleAsync connection sql expando

let executeAsync (connection: IDbConnection) (sql: string) =
    connection.ExecuteAsync(sql)
    |> Async.AwaitTask
    |> Async.Ignore

let parametrizedExecuteAsync (connection: IDbConnection) (sql: string) (param: obj) =
    connection.ExecuteAsync(sql, param)
    |> Async.AwaitTask
    |> Async.Ignore

let mapParametrizedExecuteAsync connection sql param =
    let expando = mapToExpando param
    parametrizedExecuteAsync connection sql expando

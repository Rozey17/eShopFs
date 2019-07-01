module eShop.Infrastructure.Db.Dapper

open System.Collections.Generic
open System.Data
open System.Dynamic
open Dapper
open eShop.Infrastructure.FSharp

let private mapToExpando (map: Map<string, _>) =
    let expando = ExpandoObject()
    let expandoDictionary = expando :> IDictionary<string,obj>
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

let executeAsync (connection: IDbConnection) (sql: string) =
    connection.ExecuteAsync(sql)
    |> Async.AwaitTask

let parametrizedExecuteAsync (connection: IDbConnection) (sql: string) (param: obj) =
    async {
        do!
            connection.ExecuteAsync(sql, param)
            |> Async.AwaitTask
            |> Async.Ignore
    }

let mapParametrizedExecuteAsync connection sql param =
    let expando = mapToExpando param
    parametrizedExecuteAsync connection sql expando

module eShop.Domain.Basket.CreateBasket.Api

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Npgsql
open eShop.Domain.Shared
open eShop.Domain.Basket.CreateBasket.Db
open eShop.Domain.Basket.CreateBasket.DTOs

let createBasketApi (next: HttpFunc) (ctx: HttpContext) =
    task {
        let connStr = "Server=localhost;Port=5432;Username=neet;Password=paris;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let insertBasket = insertBasketIntoDb connection

        let cmd = Command.createCommand ()
        let workflow = Implementation.createBasket insertBasket

        let! result = workflow cmd
        match result with
        | Ok basketCreated ->
            let dto = basketCreated |> BasketCreatedDTO.fromDomain
            return! json dto next ctx
        | Error err ->
            let dto = err |> DbError.fromDomain
            return! json dto next ctx
    }

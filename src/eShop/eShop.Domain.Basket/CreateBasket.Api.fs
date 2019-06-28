module eShop.Domain.Basket.CreateBasket.Api

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open eShop.Infrastructure.FSharp
open eShop.Domain.Shared
open eShop.Domain.Basket.CreateBasket.Implementation
open eShop.Domain.Basket.CreateBasket.DTOs

type Message =
    { Text: string }

let insertBasket: InsertBasket =
    fun basket ->
        AsyncResult.retn ()

let createBasketApi (next: HttpFunc) (ctx: HttpContext) =
    let cmd = Command.createCommand ()
    let workflow = Implementation.createBasket insertBasket

    task {
        let! result = workflow cmd
        match result with
        | Ok e ->
            let dto = BasketCreatedDTO.fromDomain e |> box
            let key = "Basket"
            let res = [(key, dto)] |> dict
            return! json res next ctx
        | Error err ->
            let dto = err |> DbError.fromDomain
            return! json dto next ctx
    }

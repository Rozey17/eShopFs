module internal eShop.Domain.Basket.CreateBasket.Implementation

open eShop.Infrastructure.FSharp
open eShop.Infrastructure.Db
open eShop.Domain.Shared.Types
open eShop.Domain.Basket.CommonTypes
open eShop.Domain.Basket.CreateBasket.PublicTypes

// types
type InsertBasket =
    Basket -> DbResult<Unit>

// impl
let createBasketCreatedEvent (basket: Basket) : BasketCreated = basket

let createBasket insertBasket : CreateBasket =
    fun _ ->
        asyncResult {
            let id = BasketId.create()
            let basket =
                { Id = id
                  Lines = List.empty }
            do! insertBasket basket
            let e = createBasketCreatedEvent basket

            return e
        }

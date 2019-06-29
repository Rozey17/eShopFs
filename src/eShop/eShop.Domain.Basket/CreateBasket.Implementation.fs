module internal eShop.Domain.Basket.CreateBasket.Implementation

open eShop.Infrastructure.FSharp
open eShop.Infrastructure.Db
open eShop.Domain.Shared.Types
open eShop.Domain.Basket.CommonTypes
open eShop.Domain.Basket.CreateBasket.PublicTypes

// types
type InsertBasketIntoDb =
    Basket -> DbResult<unit>

// impl
let createBasketCreatedEvent (basket: Basket) : BasketCreated = basket

let createBasket (insertBasketIntoDb: InsertBasketIntoDb) : CreateBasket =
    fun _ ->
        asyncResult {
            let id = BasketId.create()
            let basket =
                { Id = id
                  Lines = List.empty }
            do! insertBasketIntoDb basket
            let e = createBasketCreatedEvent basket

            return e
        }

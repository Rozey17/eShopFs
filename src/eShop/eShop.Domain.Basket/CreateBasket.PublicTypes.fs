module eShop.Domain.Basket.CreateBasket.PublicTypes

open eShop.Infrastructure.FSharp
open eShop.Infrastructure.Db
open eShop.Domain.Shared.Command
open eShop.Domain.Basket.CommonTypes

// input
type CreateBasketCommand = Command<Unit>

// success output
type BasketCreated = Basket

// workflow
type CreateBasket =
    CreateBasketCommand -> AsyncResult<BasketCreated, DbError>

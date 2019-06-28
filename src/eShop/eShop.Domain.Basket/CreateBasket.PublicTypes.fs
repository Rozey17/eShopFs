module eShop.Domain.Basket.CreateBasket.PublicTypes

open System
open eShop.Domain.Shared.Command
open eShop.Domain.Shared.Types

// input
type CreateBasketCommand = Command<BasketId>

// success output
type BasketCreated = BasketId
type CreateBasketEvent =
    | Created of BasketCreated

// error output
type DatabaseError = DatabaseError of Exception
type CreateBasketError =
    | Database of DatabaseError

// workflow
type CreateBasket =
    CreateBasketCommand -> Async<Result<CreateBasketEvent list, CreateBasketError>>

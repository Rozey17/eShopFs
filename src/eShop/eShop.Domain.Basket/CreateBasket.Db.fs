module eShop.Domain.Basket.CreateBasket.Db

open System
open eShop.Infrastructure.Db
open eShop.Domain.Shared.Types
open eShop.Domain.Basket.CommonTypes

type BasketDTO = { Id: Guid }

module BasketDTO =
    let fromDomain (domainObj: Basket) =
        { Id = domainObj.Id |> BasketId.value }

let insertBasketIntoDb connection basket =
    let sql = @"
        insert into
            baskets (id)
             values (@Id)"
    let dto = BasketDTO.fromDomain basket

    Dapper.parametrizedExecuteAsync connection sql dto

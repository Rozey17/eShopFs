module eShop.Domain.Basket.CreateBasket.DTOs

open System
open eShop.Domain.Shared.Types
open eShop.Domain.Basket.CreateBasket.PublicTypes

type BasketCreatedDTO =
    { Id: Guid }
module BasketCreatedDTO =
    let fromDomain (e: BasketCreated) =
        let dto =
            { Id = e.Id |> BasketId.value }
        let key = "Basket"
        [(key, dto)] |> dict

type CreateBasketErrorDTO =
    { Code: string
      Message: string }
module internal DbError =
    let fromDomain _ : CreateBasketErrorDTO =
        { Code = "500"
          Message = "Unknown Error" }
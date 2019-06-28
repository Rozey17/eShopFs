module eShop.Domain.Basket.CreateBasket.DTOs

open System
open System.Collections.Generic
open eShop.Domain.Shared.Types
open eShop.Domain.Basket.CreateBasket.PublicTypes

type BasketCreatedDTO =
    { Id: Guid }
module BasketCreatedDTO =
    let fromDomain (e: BasketCreated) : IDictionary<string, obj> =
        let obj = { Id = e.Id |> BasketId.value } |> box
        let key = "Basket"
        [(key, obj)] |> dict

type CreateBasketErrorDTO =
    { Code: string
      Message: string }
module internal DbError =
    let fromDomain _ : CreateBasketErrorDTO =
        { Code = "500"
          Message = "Unknown Error" }
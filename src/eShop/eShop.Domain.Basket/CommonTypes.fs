module eShop.Domain.Basket.CommonTypes

open eShop.Domain.Shared.Types

type OrderLine =
    { ProductId: ProductId
      ProductName: String250
      Price: Price
      Quantity: UnitQuantity }

type Address =
    { StreetAndNumber: String250
      ZipAndCity: String250
      StateOrProvince: String250
      Country: String250 }

type Basket =
    { Id: BasketId
      Lines: OrderLine list }

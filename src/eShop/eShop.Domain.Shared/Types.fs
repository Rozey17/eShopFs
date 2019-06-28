module eShop.Domain.Shared.Types

open System

/// Constrained to be 250 chars or less, not null
type String250 = private String250 of string
module String250 =
    let value (String250 v) = v
    let create =
        ConstrainedType.createString String250 250

/// Id of a basket
type BasketId = private BasketId of Guid
module BasketId =
    let value (BasketId v) = v
    let create () = BasketId (Guid.NewGuid())

/// Id of a product
type ProductId = private ProductId of Guid
module ProductId =
    let value (ProductId v) = v
    let create = ProductId (Guid.NewGuid())

/// Constrained to be a integer between 1 and 1000
type UnitQuantity = private UnitQuantity of int
module UnitQuantity =
    let value (UnitQuantity v) = v
    let create =
        ConstrainedType.createNumber UnitQuantity 0 1000

/// Constrained to be a decimal between 0.0 and 1000.00
type Price = private Price of decimal
module Price =
    let value (Price v) = v
    let create =
        ConstrainedType.createNumber Price 0.M 1000M

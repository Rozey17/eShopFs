namespace eShop.Domain.Common

open System

/// Constrained to be 250 chars or less, not null
type String250 = private String250 of string
module String250 =
    let value (String250 v) = v
    let create fieldName =
        ConstrainedType.createString fieldName String250 250
    let createOption fieldName =
        ConstrainedType.createStringOption fieldName String250 250

/// Constrained to be not null or empty
type NotEmptyString = private NotEmptyString of string
module NotEmptyString =
    let value (NotEmptyString v) = v
    let create fieldName str =
        if String.IsNullOrEmpty str then
            Error (sprintf "%s: Must not be null or empty" fieldName)
        else
            Ok (NotEmptyString str)

/// An email address
type EmailAddress = private EmailAddress of string
module EmailAddress =
    let value (EmailAddress v) = v
    let create fieldName =
        let pattern = @"[\w-]+(\.?[\w-])*\@[\w-]+(\.[\w-]+)+"
        ConstrainedType.createLike fieldName EmailAddress pattern

/// Constrained to be a integer between 1 and 10000
type UnitQuantity = private UnitQuantity of int
module UnitQuantity =
    let value (UnitQuantity v) = v
    let create fieldName =
        ConstrainedType.createNumber fieldName UnitQuantity 0 10000

/// Constrained to be a decimal between 0.0 and 50000.00
type Price = private Price of decimal
module Price =
    let value (Price v) = v
    let create fieldName =
        ConstrainedType.createNumber fieldName Price 0.M 50000M

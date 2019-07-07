module eShop.Domain.Common.ConstrainedType

open System
open System.Text.RegularExpressions

/// Create a constrained string using the constructor provided
/// Return Error if input is null, empty, or length > maxLen
let createString fieldName ctor maxLen str =
    if String.IsNullOrEmpty str then
        Error (sprintf "%s: must not be null or empty" fieldName)
    else if str.Length > maxLen then
        Error (sprintf "%s: must not be more than %i chars" fieldName maxLen)
    else
        Ok (ctor str)

let createStringOption fieldName ctor maxLen str =
    if String.IsNullOrEmpty str then
        Ok None
    else if str.Length > maxLen then
        Error (sprintf "%s: must not be more than %i chars" fieldName maxLen)
    else
        Ok (Some (ctor str))

/// Create a constrained number using the constructor provided
/// Return Error if input is less than minVal or more than maxVal
let inline createNumber fieldName ctor minVal maxVal v =
    if v < minVal then
        Error (sprintf "%s: Must not be less than %A" fieldName minVal)
    else if v > maxVal then
        Error (sprintf "%s: Must not be greater than %A" fieldName maxVal)
    else
        Ok (ctor v)

/// Create a constrained decimal<USD> using the constructor provided
/// Return Error if input is less than minVal or more than maxVal
let createCurrency<[<Measure>] 'currency, 'a> fieldName ctor minVal maxVal (d: decimal<'currency>) : Result<'a, string> =
    if d < minVal then
        Error (sprintf "%s: Must not be less than %M" fieldName minVal)
    else if d > maxVal then
        Error (sprintf "%s: Must not be greater than %M" fieldName maxVal)
    else
        Ok (ctor d)

/// Create a constrained string using the constructor provided
/// Return Error if input is null. empty, or does not match the regex pattern
let createLike fieldName ctor pattern str =
    if String.IsNullOrEmpty str then
        Error (sprintf "%s: Must not be null or empty" fieldName)
    else if Regex.IsMatch(str, pattern) then
        Ok (ctor str)
    else
        Error (sprintf "%s: '%s' must match the pattern '%s'" fieldName str pattern)

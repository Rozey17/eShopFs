namespace eShop.Domain.Conference.Web

open System
open Microsoft.AspNetCore.Http
open Giraffe

[<RequireQualifiedAccess>]
module Common =
    let private notEmpty (str: string) =
        not (String.IsNullOrEmpty str)

    let exnQueryStringValue (ctx: HttpContext) =
        let slug = ctx.TryGetQueryStringValue "slug"
        let accessCode = ctx.TryGetQueryStringValue "access_code"

        match slug, accessCode with
        | Some slug, Some accessCode when (slug |> notEmpty) && (accessCode |> notEmpty) ->
            slug, accessCode
        | _ ->
            failwith "slug or access_code is missing"

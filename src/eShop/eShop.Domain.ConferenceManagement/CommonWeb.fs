namespace eShop.Domain.ConferenceManagement.Web

open Microsoft.AspNetCore.Http
open Giraffe
open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

[<RequireQualifiedAccess>]
module CommonWeb =

    let queryStringValue (ctx: HttpContext) =
        let slug = ctx.TryGetQueryStringValue "slug" |> Option.defaultValue ""
        let accessCode = ctx.TryGetQueryStringValue "access_code" |> Option.defaultValue ""
        (slug, accessCode)

    let validate (slug, accessCode) =
        result {
            let! slug = slug |> UniqueSlug.create
            let! accessCode = accessCode |> AccessCode.create
            return (slug, accessCode)
        }

    let validateQueryStringValue ctx =
        let value = queryStringValue ctx
        validate value

    let exnQueryStringValue (ctx: HttpContext) =
        let slug = ctx.TryGetQueryStringValue "slug"
        let accessCode = ctx.TryGetQueryStringValue "access_code"
        match slug, accessCode with
        | Some slug, Some accessCode ->
            slug, accessCode
        | _ ->
            failwith "slug or access_code is missing"

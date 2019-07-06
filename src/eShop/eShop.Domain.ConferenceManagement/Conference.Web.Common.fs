namespace eShop.Domain.ConferenceManagement.Web

open Microsoft.AspNetCore.Http
open Giraffe
open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

[<RequireQualifiedAccess>]
module WebCommon =

    let getParam (ctx: HttpContext) =
        let slug = ctx.TryGetQueryStringValue "slug" |> Option.defaultValue ""
        let accessCode = ctx.TryGetQueryStringValue "access_code" |> Option.defaultValue ""
        (slug, accessCode)

    let validate (slug, accessCode) =
        result {
            let! slug = slug |> NotEditableUniqueSlug.create "Slug"
            let! accessCode = accessCode |> GeneratedAndNotEditableAccessCode.create "AccessCode"
            return (slug, accessCode)
        }

    let validateParam ctx =
        let param = getParam ctx
        validate param

    let getParamExn (ctx: HttpContext) =
        let slug = ctx.TryGetQueryStringValue "slug"
        let accessCode = ctx.TryGetQueryStringValue "access_code"
        match slug, accessCode with
        | Some slug, Some accessCode ->
            slug, accessCode
        | _ ->
            failwith "slug or access_code is missing"

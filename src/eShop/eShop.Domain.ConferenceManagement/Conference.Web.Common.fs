namespace eShop.Domain.ConferenceManagement

open Microsoft.AspNetCore.Http
open Giraffe
open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

module Web =

    module Common =

        let getParam (ctx: HttpContext) =
            let slug = ctx.TryGetQueryStringValue "slug" |> Option.defaultValue ""
            let accessCode = ctx.TryGetQueryStringValue "access_code" |> Option.defaultValue ""
            (slug, accessCode)

        let validateParam (slug, accessCode) =
            result {
                let! slug = slug |> NotEditableUniqueSlug.create "Slug"
                let! accessCode = accessCode |> GeneratedAndNotEditableAccessCode.create "AccessCode"
                return (slug, accessCode)
            }

        let validateContext ctx =
            let param = getParam ctx
            validateParam param

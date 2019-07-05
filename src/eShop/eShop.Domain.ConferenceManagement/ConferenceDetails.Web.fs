module eShop.Domain.ConferenceManagement.ConferenceDetails.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

let parseParams slugStr accessCodeStr =
    result {
        let! slug = slugStr |> NotEditableUniqueSlug.create "Slug"
        let! accessCode = accessCodeStr |> GeneratedAndNotEditableAccessCode.create "AccessCode"

        return (slug, accessCode)
    }

// get
let renderConferenceDetailsView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slugStr = ctx.TryGetQueryStringValue "slug" |> Option.defaultValue ""
        let accessCodeStr = ctx.TryGetQueryStringValue "access_code" |> Option.defaultValue ""

        match parseParams slugStr accessCodeStr with
        | Ok (slug, accessCode) ->
            let! dto = Db.ReadConferenceDetails.query connection slug accessCode
            match dto with
            | Some dto ->
                let viewData = dict [("Slug", box dto.Slug); ("AccessCode", box dto.AccessCode)]
                return! razorHtmlView "ConferenceDetails" (Some dto) (Some viewData) None next ctx
            | None ->
                return! text "not found" next ctx
        | Error _ ->
            return! text "bad request" next ctx
    }

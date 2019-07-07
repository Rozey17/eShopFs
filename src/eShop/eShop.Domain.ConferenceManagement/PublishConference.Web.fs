module eShop.Domain.ConferenceManagement.PublishConference.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Npgsql
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common
open eShop.Domain.ConferenceManagement.Db
open eShop.Domain.ConferenceManagement.Web

// post
let publishConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let values = CommonWeb.validateQueryStringValue ctx
        match values with
        | Ok (slug, accessCode) ->
            let cmd = Command.create (slug, accessCode)

            let readSingleConference = CommonDb.ReadSingleConference.execute connection
            let markConferenceAsPublishedInDb = Db.MarkConferenceAsPublishedInDb.execute connection
            let workflow = Impl.publishConference readSingleConference markConferenceAsPublishedInDb

            let! result = workflow cmd
            match result with
            | [ (ConferencePublished (PublishedConference info)) ] ->
                let slug = info.Slug |> NotEditableUniqueSlug.value
                let accessCode = info.AccessCode |> GeneratedAndNotEditableAccessCode.value
                let url = sprintf "/conferences/details?slug=%s&access_code=%s" slug accessCode
                return! redirectTo false url next ctx

            | _ ->
                return! text "unknown error" next ctx

        | Error _ ->
            return! text "bad request" next ctx
    }

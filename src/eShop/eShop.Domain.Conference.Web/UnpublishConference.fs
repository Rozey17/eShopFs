module eShop.Domain.Conference.Web.UnpublishConference.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Npgsql
open eShop.Domain.Conference
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.UnpublishConference

// post
let unpublishConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx
        let cmd = ConferenceIdentifier (slug, accessCode)

        let readSingleConference = ConferenceDb.Impl.ReadSingleConference.query connection
        let markConferenceAsPublishedInDb = ConferenceDb.Impl.MarkConferenceAsUnpublished.execute connection
        let workflow = Impl.unpublishConference readSingleConference markConferenceAsPublishedInDb

        let! result = workflow cmd

        match result with
        | Ok [ (ConferenceUnpublished _) ] ->
            let url = sprintf "/conferences/details?slug=%s&access_code=%s" slug accessCode
            return! redirectTo false url next ctx
        | _ ->
            return! text "unknown error" next ctx
    }

module eShop.Domain.Conference.Web.PublishConference.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Npgsql
open eShop.Infrastructure.Bus
open eShop.Domain.Conference
open eShop.Domain.Conference.PublishConference
open eShop.Domain.Conference.Web

// post
let publishConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx
        let cmd = ConferenceIdentifier (slug, accessCode)

        let readSingleConference = ConferenceDb.Impl.ReadSingleConference.query connection
        let markConferenceAsPublished = ConferenceDb.Impl.MarkConferenceAsPublished.execute connection
        let workflow = Impl.publishConference readSingleConference markConferenceAsPublished

        let! result = workflow cmd

        match result with
        | Ok [ (ConferencePublished e) ] ->
            // internal response
            let e' = ConferencePublishedDTO.fromDomain e
            do! Bus.Publish e'

            // web response
            let url = sprintf "/conferences/details?slug=%s&access_code=%s" slug accessCode
            return! redirectTo false url next ctx
        | _ ->
            return! text "unknown error" next ctx
    }
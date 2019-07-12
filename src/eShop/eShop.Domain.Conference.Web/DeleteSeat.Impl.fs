module eShop.Domain.Conference.Web.DeleteSeat.Impl

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe
open Npgsql
open eShop.Domain.Conference
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails
open eShop.Domain.Conference.DeleteSeat

// post
let deleteSeat next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"
        let id = Common.exnQueryStringValue ctx "id" |> Guid
        let! detailsResult = Db.readConferenceDetails connection (slug, accessCode)

        match detailsResult with
        | Ok details ->
            let cmd = details.Id, id

            let readSingleConferenceFromDb = ConferenceDb.Impl.ReadSingleConference.query connection
            let deleteSeatInDb = ConferenceDb.Impl.DeleteSeat.execute connection
            let workflow = Impl.deleteSeat readSingleConferenceFromDb deleteSeatInDb

            let! result = workflow cmd

            match result with
            | Ok _ -> // TODO: publish events
                return! text "ok" next ctx

            | _ ->
                return! text "bad request" next ctx

        | _ ->
            return! text "bad request" next ctx
    }

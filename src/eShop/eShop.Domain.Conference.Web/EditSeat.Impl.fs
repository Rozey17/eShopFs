module eShop.Domain.Conference.Web.EditSeat.Impl

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Infrastructure.Bus
open eShop.Domain.Conference
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails
open eShop.Domain.Conference.ReadModel.ReadSeatType

// get
let renderEditSeatView next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"
        let! detailsResult = Db.readConferenceDetails connection (slug, accessCode)

        match detailsResult with
        | Ok details ->
            let id = Common.exnQueryStringValue ctx "id" |> Guid
            let! seatTypeResult = Db.readSeatType connection (details.Id, id)

            match seatTypeResult with
            | Ok seatType ->
                let form = SeatTypeFormDTO.fromSeatTypeDTO seatType
                return! razorHtmlView "EditSeat" (Some form) None None next ctx

            | _ ->
                return! text "unknown error" next ctx

        | _ ->
            return! text "bad request" next ctx
    }


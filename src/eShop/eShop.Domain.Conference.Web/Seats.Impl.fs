module eShop.Domain.Conference.Web.Seats.Impl

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails
open eShop.Domain.Conference.ReadModel.ReadSeat
open eShop.Domain.Conference.ReadModel.ReadSeats

// get
let renderSeatsView next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"
        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok details ->
            let! seats = Db.readSeats connection details.Id
            let viewData = dict [
                ("Slug", box details.Slug)
                ("AccessCode", box details.AccessCode)
                ("OwnerName", box details.OwnerName)
                ("WasEverPublished", box details.WasEverPublished)
            ]
            return! razorHtmlView "Seats" (Some seats) (Some viewData) None next ctx

        | _ ->
            return! text "bad request" next ctx
    }

// get
let renderSeatRowView next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"
        let id = Common.exnQueryStringValue ctx "id" |> Guid
        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok details ->
            let! seatResult = Db.readSeat connection (details.Id, id)

            match seatResult with
            | Ok seat ->
                let viewData = dict [("WasEverPublished", box details.WasEverPublished)]
                return! razorHtmlView "SeatGrid" (Some [seat]) (Some viewData) None next ctx

            | _ ->
                return! text "bad request" next ctx

        | _ ->
            return! text "bad request" next ctx
    }
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
open eShop.Domain.Conference.ReadModel.ReadSeat
open eShop.Domain.Conference.UpdateSeat

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
            let! seatTypeResult = Db.readSeat connection (details.Id, id)

            match seatTypeResult with
            | Ok seatType ->
                let form = SeatTypeFormDTO.fromReadModel seatType
                return! razorHtmlView "EditSeat" (Some form) None None next ctx

            | _ ->
                return! text "unknown error" next ctx

        | _ ->
            return! text "bad request" next ctx
    }

// post
let updateSeat next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"
        let! detailsResult = Db.readConferenceDetails connection (slug, accessCode)

        match detailsResult with
        | Ok details ->
            let! form = ctx.BindFormAsync<SeatTypeFormDTO>()
            let unvalidatedSeat = SeatTypeFormDTO.toUnvalidatedSeatType form
            let cmd = unvalidatedSeat

            let readSingleConferenceFromDb = ConferenceDb.Impl.ReadSingleConference.query connection
            let updateSeatInDb = ConferenceDb.Impl.UpdateSeat.execute connection
            let workflow = Impl.updateSeat readSingleConferenceFromDb updateSeatInDb

            let! result = workflow cmd

            match result with
            | Ok [ SeatUpdated e ] ->
                let dto = SeatTypeDTO.fromDomain e
                let viewData = dict [("WasEverPublished", box details.WasEverPublished)]
                return! razorHtmlView "SeatGrid" (Some [dto]) (Some viewData) None next ctx

            | Ok [ SeatUpdated e1; PublishedSeatUpdated e2 ] ->
                // to registration context
                let dto = PublishedSeatUpdatedDTO.fromDomain e2
                do! Bus.Publish dto

                // web response
                let dto = SeatTypeDTO.fromDomain e1
                let viewData = dict [("WasEverPublished", box details.WasEverPublished)]
                return! razorHtmlView "SeatGrid" (Some [dto]) (Some viewData) None next ctx

            | Error (Validation (ValidationError err)) ->
                let modelState = ModelStateDictionary()
                if err.StartsWith "Name" then
                    modelState.AddModelError("Name", err)
                else if err.StartsWith "Description" then
                    modelState.AddModelError("Description", err)
                else if err.StartsWith "Quantity" then
                    modelState.AddModelError("Quantity", err)
                else if err.StartsWith "Price" then
                    modelState.AddModelError("Price", err)
                return! razorHtmlView "EditSeat" (Some form) None (Some modelState) next ctx

            | _ ->
                return! text "unknown error" next ctx

        | _ ->
            return! text "bad request" next ctx
    }

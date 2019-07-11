module eShop.Domain.Conference.Web.CreateSeat.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails
open eShop.Domain.Conference.CreateSeat

// get
let renderCreateSeatView next ctx =
    razorHtmlView "CreateSeat" None None None next ctx

// post
let createSeat next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx
        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok details ->
            let! form = ctx.BindFormAsync<SeatTypeFormDTO>()
            let unvalidatedSeatType = SeatTypeFormDTO.toUnvalidatedSeatType details.Id form
            let cmd = unvalidatedSeatType

            let readSingleConference = ConferenceDb.Impl.ReadSingleConference.query connection
            let insertSeatType = ConferenceDb.Impl.InsertSeatType.execute connection
            let workflow = Impl.createSeat readSingleConference insertSeatType

            let! result = workflow cmd
            match result with
            | Ok events ->
                return! razorHtmlView "SeatGrid" None None None next ctx

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
                return! razorHtmlView "CreateSeat" (Some form) None (Some modelState) next ctx

            | _ ->
                return! text "unknown error" next ctx

        | _ ->
            return! text "bad request" next ctx
    }


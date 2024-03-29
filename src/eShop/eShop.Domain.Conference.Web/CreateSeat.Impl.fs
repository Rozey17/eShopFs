module eShop.Domain.Conference.Web.CreateSeat.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Infrastructure.Bus
open eShop.Domain.Conference
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails
open eShop.Domain.Conference.CreateSeat

// get
let renderCreateSeatView next ctx =
    let form =
        { Name = ""
          Description = ""
          Quantity = 0
          Price = 0M }
    razorHtmlView "CreateSeat" (Some form) None None next ctx

// post
let createSeat next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"
        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok details ->
            let! form = ctx.BindFormAsync<SeatTypeFormDTO>()
            let unvalidatedSeatType = SeatTypeFormDTO.toUnvalidatedSeatType details.Id form
            let cmd = unvalidatedSeatType

            let readSingleConference = ConferenceDb.Impl.ReadSingleConference.query connection
            let insertSeat = ConferenceDb.Impl.InsertSeat.execute connection
            let workflow = Impl.createSeat readSingleConference insertSeat

            let! result = workflow cmd

            match result with
            | Ok [ SeatCreated e ] ->
                let dto = SeatTypeDTO.fromDomain e
                let viewData = dict [("WasEverPublished", box details.WasEverPublished)]
                return! razorHtmlView "SeatGrid" (Some [dto]) (Some viewData) None next ctx

            | Ok [ SeatCreated e1; PublishedSeatCreated e2 ] ->
                // to registration context
                let dto = PublishedSeatCreatedDTO.fromDomain e2
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
                return! razorHtmlView "CreateSeat" (Some form) None (Some modelState) next ctx

            | _ ->
                return! text "unknown error" next ctx

        | _ ->
            return! text "bad request" next ctx
    }


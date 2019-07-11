module eShop.Domain.Conference.Web.CreateConference.Impl

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Infrastructure.Bus
open eShop.Domain.Conference
open eShop.Domain.Conference.CreateConference

// get
let renderCreateConferenceView: HttpHandler =
    let form: ConferenceFormDTO =
        { OwnerName = ""
          OwnerEmail = ""
          Slug = ""
          Name = ""
          Tagline = ""
          Location = ""
          TwitterSearch = ""
          Description = ""
          StartDate = DateTime.Now.AddDays(1.)
          EndDate = DateTime.Now.AddDays(2.) }

    razorHtmlView "CreateConference" (Some form) None None

// post
let createConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let! form = ctx.BindFormAsync<ConferenceFormDTO>()
        let unvalidatedInfo = form |> ConferenceFormDTO.toUnvalidatedConferenceInfo
        let cmd = unvalidatedInfo

        let checkSlugExists = ConferenceDb.Impl.CheckSlugExists.execute connection
        let insertConferenceIntoDb = ConferenceDb.Impl.InsertConference.execute connection
        let workflow = Impl.createConference checkSlugExists insertConferenceIntoDb

        let! result = workflow cmd

        match result with
        | Ok [ (ConferenceCreated e) ] ->
            // to registration context
            let dto = ConferenceCreatedDTO.fromDomain e
            do! Bus.Publish dto

            // web response
            let info = e |> Conference.info
            let slug = info.Slug |> NotEditableUniqueSlug.value
            let accessCode = info.AccessCode |> GeneratedAndNotEditableAccessCode.value
            let url = sprintf "/conferences/details?slug=%s&access_code=%s" slug accessCode
            return! redirectTo false url next ctx

        | Error (Validation (ValidationError error)) ->
            let modelState = ModelStateDictionary()
            modelState.AddModelError("", error)

            return! razorHtmlView "CreateConference" (Some form) None (Some modelState) next ctx

        | _ ->
            return! text "Unknown Error" next ctx
    }

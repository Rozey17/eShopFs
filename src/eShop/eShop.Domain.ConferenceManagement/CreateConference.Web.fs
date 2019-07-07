module eShop.Domain.ConferenceManagement.CreateConference.Web

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

// get
let renderCreateConferenceView: HttpHandler =
    let form =
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
        let cmd = Command.create unvalidatedInfo

        let checkSlugExists = Db.CheckSlugExists.query connection
        let insertConferenceIntoDb = Db.InsertConferenceIntoDb.execute connection
        let workflow = Impl.createConference checkSlugExists insertConferenceIntoDb

        let! result = workflow cmd

        match result with
        | Ok [ (ConferenceCreated (UnpublishedConference(info, _))) ] ->
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

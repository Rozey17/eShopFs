module eShop.Domain.ConferenceManagement.CreateConference.Web

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Database

// get
let renderCreateConferenceView next ctx =
    let form =
        { OwnerName = ""
          OwnerEmail = ""
          Slug = ""
          Name = ""
          Tagline = ""
          Location = ""
          TwitterSearch = ""
          Description = ""
          StartDate = DateTime.Now
          EndDate = DateTime.Now.AddDays(1.) }
    razorHtmlView "CreateConference" (Some form) None None next ctx

// post
let createConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let! form = ctx.BindFormAsync<ConferenceFormDTO>()
        let unvalidatedInfo = form |> ConferenceFormDTO.toUnvalidatedConferenceInfo
        let cmd = Command.create unvalidatedInfo

        let checkSlugExists = DatabaseImplementation.checkSlugExists connection
        let insertConferenceIntoDb = DatabaseImplementation.insertConferenceIntoDb connection
        let workflow = Implementation.createConference checkSlugExists insertConferenceIntoDb

        let! result = workflow cmd

        match result with
        | Ok [ (ConferenceCreated event) ] ->
            return! razorHtmlView "CreateConference" (Some form) None None next ctx
        | Error (Validation error) ->
            return! razorHtmlView "CreateConference" (Some form) None None next ctx
        | _ ->
            return! text "Unknown Error" next ctx
    }

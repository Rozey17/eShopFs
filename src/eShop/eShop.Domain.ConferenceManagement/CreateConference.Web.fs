module eShop.Domain.ConferenceManagement.CreateConference.Web

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open eShop.Domain.Shared

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
let checkSlugExists: Implementation.CheckSlugExists =
    fun _ ->
        async {
            return true
        }

let insertConferenceIntoDb: Implementation.InsertConferenceIntoDb =
    fun _ ->
        async {
            return ()
        }

let createConference next (ctx: HttpContext) =
    task {
        let! form = ctx.BindFormAsync<ConferenceFormDTO>()
        let unvalidatedInfo = form |> ConferenceFormDTO.toUnvalidatedConferenceInfo

        let cmd = Command.createCommand unvalidatedInfo
        let workflow = Implementation.createConference checkSlugExists insertConferenceIntoDb
        let! result = workflow cmd

        printfn "%A" result

        match result with
        | Ok events ->
            return! razorHtmlView "CreateConference" (Some form) None None next ctx
        | Error error ->
            return! razorHtmlView "CreateConference" (Some form) None None next ctx
    }

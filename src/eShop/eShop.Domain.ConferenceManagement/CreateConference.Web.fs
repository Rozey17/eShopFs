module eShop.Domain.ConferenceManagement.CreateConference.Web

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor

let renderCreateConferenceView next ctx =
    let viewModel =
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
    razorHtmlView "CreateConference" (Some viewModel) None None next ctx

let createConference next (ctx: HttpContext) =
    task {
        let! conferenceForm = ctx.BindFormAsync<ConferenceFormDTO>()
        let unvalidatedConferenceInfo = conferenceForm |> ConferenceFormDTO.toUnvalidatedConferenceInfo

        return! razorHtmlView "CreateConference" (Some conferenceForm) None None next ctx
    }

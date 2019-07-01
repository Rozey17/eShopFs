module eShop.Domain.ConferenceManagement.CreateConference.Web

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor

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

let createConference next (ctx: HttpContext) =
    task {
        let! form = ctx.BindFormAsync<ConferenceFormDTO>()
        let unvalidatedInfo = form |> ConferenceFormDTO.toUnvalidatedConferenceInfo

        return! razorHtmlView "CreateConference" (Some form) None None next ctx
    }

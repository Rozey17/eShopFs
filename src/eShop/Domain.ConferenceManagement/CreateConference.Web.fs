module eShop.Domain.ConferenceManagement.CreateConference.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor

open eShop.Domain.ConferenceManagement.CreateConference.ViewModel

let renderCreateReferenceView next (ctx: HttpContext) =
    task {
        let modelState = ModelStateDictionary()
        let! model = ctx.BindModelAsync<CreateConferenceViewModel>()
        return! razorHtmlView "CreateConference" (Some model) None (Some modelState) next ctx
    }


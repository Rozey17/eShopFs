module eShop.Domain.Conference.Web.LocateConference.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference.ReadModel.LocateConference

[<CLIMutable>]
type LocateFormDTO =
    { Email: string
      AccessCode: string }

module LocateFormDTO =

    let toConferenceIdentifier (form: LocateFormDTO) =
        let domainObj: ConferenceIdentifier =
            { Email = form.Email
              AccessCode = form.AccessCode }
        domainObj

// get
let renderLocateConferenceView: HttpHandler =
    razorHtmlView "LocateConference" None None None

// post
let locateConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let! form = ctx.BindFormAsync<LocateFormDTO>()
        let identifier = LocateFormDTO.toConferenceIdentifier form

        let! result = Db.locateConference connection identifier

        match result with
        | Ok conference ->
            let url = sprintf "/conferences/details?slug=%s&access_code=%s" conference.Slug conference.AccessCode
            return! redirectTo false url next ctx

        | _ ->
            let modelState = ModelStateDictionary()
            modelState.AddModelError("", "Could not locate a conference with the provided email and access code.")
            let viewData = dict [("Email", box form.Email)]
            return! razorHtmlView "LocateConference" None (Some viewData) (Some modelState) next ctx
    }

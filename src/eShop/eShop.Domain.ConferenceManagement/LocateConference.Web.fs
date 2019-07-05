module eShop.Domain.ConferenceManagement.LocateConference.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Infrastructure
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Common

// get
let renderLocateConferenceView: HttpHandler =
    razorHtmlView "LocateConference" None None None

let validateForm (form: LocateFormDTO) =
    result {
        let! email = form.Email |> EmailAddress.create "Email"
        let! accessCode = form.AccessCode |> GeneratedAndNotEditableAccessCode.create "Access Code"
        let validatedForm = {| Email = email; AccessCode = accessCode |}

        return validatedForm
    }
// post
let locateConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let! form = ctx.BindFormAsync<LocateFormDTO>()
        let validateResult = validateForm form

        match validateResult with
        | Ok validatedForm ->
            let! dto = Db.ReadConferenceByEmailAndAccessCode.query connection validatedForm.Email validatedForm.AccessCode
            match dto with
            | Some dto ->
                let url = sprintf "/conferences/details?slug=%s&access_code=%s" dto.Slug dto.AccessCode
                return! redirectTo false url next ctx

            | None ->
                let modelState = ModelStateDictionary()
                modelState.AddModelError("", "Could not locate a conference with the provided email and access code.")
                let viewData = dict [("Email", box form.Email)]

                return! razorHtmlView "LocateConference" None (Some viewData) (Some modelState) next ctx

        | Error error ->
            let modelState = ModelStateDictionary()
            modelState.AddModelError("", error)
            let viewData = dict [("Email", box form.Email)]

            return! razorHtmlView "LocateConference" None (Some viewData) (Some modelState) next ctx
    }

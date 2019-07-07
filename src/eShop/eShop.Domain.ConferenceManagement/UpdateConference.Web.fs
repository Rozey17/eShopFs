module eShop.Domain.ConferenceManagement.UpdateConference.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Web
open eShop.Domain.ConferenceManagement.EditConference

// post
let updateConference next (ctx: HttpContext) =
    task {
        let slug, accessCode = CommonWeb.exnQueryStringValue ctx

        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let! form = ctx.BindFormAsync<EditConferenceFormDTO>()
        let unvalidatedInfo = form |> EditConferenceFormDTO.toUnvalidatedConferenceInfo
        let cmd = Command.create unvalidatedInfo

        let updateConferenceInfoInDb = Db.UpdateConferenceInDb.execute connection
        let workflow = Impl.updateConference updateConferenceInfoInDb

        let! result = workflow cmd

        match result with
        | Ok [ (ConferenceUpdated _) ] ->
            let url = sprintf "/conferences/details?slug=%s&access_code=%s" slug accessCode

            return! redirectTo false url next ctx

        | Error (Validation (ValidationError error)) ->
            let modelState = ModelStateDictionary()
            modelState.AddModelError("", error)

            let viewData = dict [("Slug", box slug); ("AccessCode", box accessCode)]

            return! razorHtmlView "EditConference" (Some form) (Some viewData) (Some modelState) next ctx

        | _ ->
            return! text "Unknown Error" next ctx
    }

module eShop.Domain.Conference.Web.EditConference.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.UpdateConference
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails

// get
let renderEditConferenceView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"

        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok details ->
            let viewData = dict [
                ("Slug", box details.Slug)
                ("AccessCode", box details.AccessCode)
            ]
            let form = EditConferenceFormDTO.fromConferenceDetailsDTO details
            return! razorHtmlView "EditConference" (Some form) (Some viewData) None next ctx
        | _ ->
            return! text "bad request" next ctx
    }

// post
let updateConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = Common.exnQueryStringValue ctx "slug"
        let accessCode = Common.exnQueryStringValue ctx "access_code"

        let! form = ctx.BindFormAsync<EditConferenceFormDTO>()
        let unvalidatedInfo = form |> EditConferenceFormDTO.toUnvalidatedConferenceInfo
        let cmd = unvalidatedInfo

        let readSingleConference = ConferenceDb.Impl.ReadSingleConference.query connection
        let updateConference = ConferenceDb.Impl.UpdateConference.execute connection
        let workflow = Impl.updateConference readSingleConference updateConference

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
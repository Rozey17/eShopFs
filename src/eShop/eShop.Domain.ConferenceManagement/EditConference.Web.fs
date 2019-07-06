module eShop.Domain.ConferenceManagement.EditConference.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.ConferenceManagement

// get
let renderEditConferenceView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        match Web.Common.validateParam ctx with
        | Ok (slug, accessCode) ->
            let! result = ConferenceDetails.Db.ReadConferenceDetails.query connection slug accessCode
            match result with
            | Some details ->
                let form = ConferenceFormDTO.fromConferenceDetailsDTO details
                let viewData = dict [("Slug", box form.Slug); ("AccessCode", box form.AccessCode)]
                return! razorHtmlView "EditConference" (Some form) (Some viewData) None next ctx

            | None ->
                return! text "not found" next ctx

        | Error _ ->
            return! text "bad request" next ctx
    }

// post
let updateConference next (ctx: HttpContext) =
    task {
        let! form = ctx.BindFormAsync<ConferenceFormDTO>()
        printfn "%A" form

        let viewData = dict [("Slug", box form.Slug); ("AccessCode", box form.AccessCode)]

        return! razorHtmlView "EditConference" (Some form) (Some viewData) None next ctx
    }

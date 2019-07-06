module eShop.Domain.ConferenceManagement.EditConference.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.ConferenceManagement.Web

// get
let renderEditConferenceView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        match WebCommon.validateParam ctx with
        | Ok (slug, accessCode) ->
            let! result = Db.ReadConferenceDetails.query connection slug accessCode
            match result with
            | Some details ->
                let viewData = dict [("Slug", box details.Slug); ("AccessCode", box details.AccessCode)]
                return! razorHtmlView "EditConference" (Some details) (Some viewData) None next ctx

            | None ->
                return! text "not found" next ctx

        | Error _ ->
            return! text "bad request" next ctx
    }

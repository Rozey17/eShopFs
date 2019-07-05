module eShop.Domain.ConferenceManagement.EditConference.Web

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.ConferenceManagement
open eShop.Domain.ConferenceManagement.ConferenceDetails

let renderEditConferenceView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        match Web.Common.validateParam ctx with
        | Ok (slug, accessCode) ->
            let! dto = Db.ReadConferenceDetails.query connection slug accessCode
            match dto with
            | Some dto ->
                let viewData = dict [("Slug", box dto.Slug); ("AccessCode", box dto.AccessCode)]
                return! razorHtmlView "EditConference" (Some dto) (Some viewData) None next ctx

            | None ->
                return! text "not found" next ctx

        | Error _ ->
            return! text "bad request" next ctx
    }

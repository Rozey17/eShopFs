module eShop.Domain.Conference.Web.ConferenceDetails

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails

// get
let renderConferenceDetailsView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx
        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok dto ->
            let viewData = dict [("Slug", box dto.Slug); ("AccessCode", box dto.AccessCode)]
            return! razorHtmlView "ConferenceDetails" (Some dto) (Some viewData) None next ctx
        | _ ->
            return! text "bad request" next ctx
    }

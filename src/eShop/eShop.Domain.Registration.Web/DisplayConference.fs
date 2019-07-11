module eShop.Domain.Registration.Web.DisplayConference.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Registration.Conference.ReadModel.ReadConferenceDetails

let renderDisplayConferenceView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug = ctx.TryGetQueryStringValue "slug" |> Option.defaultValue ""

        let! queryResult = Db.readConferenceDetails connection slug
        match queryResult with
        | Ok details ->
            return! razorHtmlView "DisplayConference" (Some details) None None next ctx
        | Error RecordNotFound ->
            return! text "not found" next ctx
    }

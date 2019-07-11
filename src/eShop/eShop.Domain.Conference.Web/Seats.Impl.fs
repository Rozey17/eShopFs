module eShop.Domain.Conference.Web.Seats.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails

let renderSeatsView next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx
        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok details ->
            let viewData = dict [
                ("Slug", box details.Slug)
                ("AccessCode", box details.AccessCode)
                ("OwnerName", box details.OwnerName)
            ]
            return! razorHtmlView "Seats" None (Some viewData) None next ctx
        | _ ->
            return! text "bad request" next ctx
    }

module eShop.Domain.Registration.Web.RegistrationHome.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe.Razor
open Npgsql
open eShop.Domain.Registration.Conference.ReadModel.ReadPublishedConferences

// get
let renderHomeView next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let! conferences = Db.readPublishedConferences connection ()
        return! razorHtmlView "RegistrationHome" (Some conferences) None None next ctx
    }
module eShop.Domain.Conference.Web.SeatGrid.Impl

open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.ReadModel.ReadSeats

let renderSeatGridView next ctx =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx
        let! seats = Db.readSeats connection (slug, accessCode)

        return! razorHtmlView "SeatGrid" (Some seats) None None next ctx
    }

module eShop.Domain.Conference.Web.CreateSeat.Impl

open Giraffe.Razor

let renderCreateSeatView next ctx =
    razorHtmlView "CreateSeat" None None None next ctx


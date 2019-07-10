module eShop.Domain.Conference.Web.Home

open Giraffe
open Giraffe.Razor

// get
let renderHomeView: HttpHandler =
    razorHtmlView "Home" None None None

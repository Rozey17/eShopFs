module eShop.Domain.Conference.Web.Home.Impl

open Giraffe
open Giraffe.Razor

// get
let renderHomeView: HttpHandler =
    razorHtmlView "Home" None None None

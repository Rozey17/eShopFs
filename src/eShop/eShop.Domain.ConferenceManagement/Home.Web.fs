module eShop.Domain.ConferenceManagement.Home.Web

open Giraffe
open Giraffe.Razor

// get
let renderHomeView: HttpHandler =
    razorHtmlView "Home" None None None
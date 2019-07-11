module eShop.Domain.Conference.Web.ConferencesHome.Impl

open Giraffe
open Giraffe.Razor

// get
let renderHomeView: HttpHandler =
    razorHtmlView "ConferencesHome" None None None

module eShop.Domain.ConferenceManagement.LocateConference.Web

open Giraffe
open Giraffe.Razor

// get
let renderLocateConferenceView: HttpHandler =
    razorHtmlView "LocateConference" None None None

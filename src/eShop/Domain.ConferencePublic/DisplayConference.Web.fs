module eShop.Domain.ConferencePublic.DisplayConference.Web

open Giraffe.Razor

let renderDisplayConferenceView next ctx =
    razorHtmlView "DisplayConference" None None None next ctx

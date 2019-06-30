module eShop.Domain.ConferenceManagement.Conference.CreateConference.Web

open Giraffe.Razor

let renderCreateReferenceView next ctx =
    razorHtmlView "CreateConference" None None None next ctx


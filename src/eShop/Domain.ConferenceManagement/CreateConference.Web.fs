module eShop.Domain.ConferenceManagement.CreateConference.Web

open Giraffe.Razor

let renderCreateReferenceView next ctx =
    razorHtmlView "CreateConference" None None None next ctx


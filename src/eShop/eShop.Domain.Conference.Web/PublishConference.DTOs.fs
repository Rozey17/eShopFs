namespace eShop.Domain.Conference.Web.PublishConference

open System
open eShop.Domain.Conference
open eShop.Domain.Conference.PublishConference

type ConferencePublishedDTO =
    { Id: Guid }

module ConferencePublishedDTO =

    let fromDomain (e: ConferencePublished) =
        { Id = e |> Conference.id |> ConferenceId.value }

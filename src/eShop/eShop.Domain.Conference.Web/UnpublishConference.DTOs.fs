namespace eShop.Domain.Conference.Web.UnpublishConference

open System
open eShop.Domain.Conference
open eShop.Domain.Conference.UnpublishConference

type ConferenceUnpublishedDTO = { Id: Guid }
module ConferenceUnpublishedDTO =
    let fromDomain (e: ConferenceUnpublished) =
        { Id = e |> Conference.id |> ConferenceId.value }

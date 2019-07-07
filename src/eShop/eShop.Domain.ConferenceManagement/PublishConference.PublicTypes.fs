namespace eShop.Domain.ConferenceManagement.PublishConference

open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

// input
type PublishConferenceCommand = Command<UniqueSlug * AccessCode>


// success output
type ConferencePublished = PublishedConference
type PublishConferenceEvent =
    | ConferencePublished of ConferencePublished

// workflow
type PublishConference =
    PublishConferenceCommand -> Async<PublishConferenceEvent list>

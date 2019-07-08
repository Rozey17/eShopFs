namespace eShop.Domain.ConferenceManagement.UnpublishConference

open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

// input
type UnpublishConferenceCommand = Command<UniqueSlug * AccessCode>


// success output
type ConferenceUnpublished = UnpublishedConference
type UnpublishConferenceEvent =
    | ConferenceUnpublished of ConferenceUnpublished

// workflow
type UnpublishConference =
    UnpublishConferenceCommand -> Async<UnpublishConferenceEvent list>
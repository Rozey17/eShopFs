namespace eShop.Domain.Conference.UnpublishConference

open System
open eShop.Infrastructure
open eShop.Domain.Conference

// input
type ConferenceId = Guid
type UnpublishConferenceCommand = ConferenceId

// success output
type ConferenceUnpublished = Conference
type UnpublishConferenceEvent =
    | ConferenceUnpublished of ConferenceUnpublished

// error output
type ValidationError = ValidationError of string
type UnpublishConferenceError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.RecordNotFound

// workflow
type UnpublishConference =
    UnpublishConferenceCommand -> AsyncResult<UnpublishConferenceEvent list, UnpublishConferenceError>

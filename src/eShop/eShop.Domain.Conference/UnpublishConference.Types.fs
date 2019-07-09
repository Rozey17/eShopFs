namespace eShop.Domain.Conference.UnpublishConference

open eShop.Infrastructure
open eShop.Domain.Conference

// input
type ConferenceIdentifier = ConferenceIdentifier of slug:string * accessCode:string
type UnpublishConferenceCommand = ConferenceIdentifier

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

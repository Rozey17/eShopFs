namespace eShop.Domain.Conference.PublishConference

open eShop.Infrastructure
open eShop.Domain.Conference

// input
type ConferenceIdentifier = ConferenceIdentifier of slug:string * accessCode:string
type PublishConferenceCommand = ConferenceIdentifier

// success output
type ConferencePublished = Conference
type PublishConferenceEvent =
    | ConferencePublished of ConferencePublished

// error output
type ValidationError = ValidationError of string
type PublishConferenceError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.NotFound

// workflow
type PublishConference =
    PublishConferenceCommand -> AsyncResult<PublishConferenceEvent list, PublishConferenceError>

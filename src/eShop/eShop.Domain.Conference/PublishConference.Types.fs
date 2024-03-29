namespace eShop.Domain.Conference.PublishConference

open System
open eShop.Infrastructure
open eShop.Domain.Conference

// input
type ConferenceId = Guid
type PublishConferenceCommand = ConferenceId

// success output
type ConferencePublished = Conference
type PublishedSeatCreated = SeatType
type PublishConferenceEvent =
    | ConferencePublished of ConferencePublished
    | PublishedSeatCreated of PublishedSeatCreated

// error output
type ValidationError = ValidationError of string
type PublishConferenceError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.RecordNotFound

// workflow
type PublishConference =
    PublishConferenceCommand -> AsyncResult<PublishConferenceEvent list, PublishConferenceError>

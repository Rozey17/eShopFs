namespace eShop.Domain.Conference.UpdateConference

open System
open eShop.Infrastructure
open eShop.Domain.Conference

// input
type UnvalidatedConferenceInfo =
    { Id: Guid
      Name: string
      Tagline: string
      Location: string
      TwitterSearch: string
      Description: string
      StartDate: DateTime
      EndDate: DateTime }

type UpdateConferenceCommand = UnvalidatedConferenceInfo

// success output

type ConferenceUpdated = Conference
type UpdateConferenceEvent =
    | ConferenceUpdated of ConferenceUpdated

// error output
type ValidationError = ValidationError of string
type UpdateConferenceError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.RecordNotFound

// workflow
type UpdateConference =
    UpdateConferenceCommand -> AsyncResult<UpdateConferenceEvent list, UpdateConferenceError>


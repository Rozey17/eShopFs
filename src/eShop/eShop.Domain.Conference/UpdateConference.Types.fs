namespace eShop.Domain.Conference.UpdateConference

open System
open eShop.Infrastructure
open eShop.Domain.Conference

// input
type ConferenceIdentifier = ConferenceIdentifier of slug:string * accessCode:string

type UnvalidatedConferenceInfo =
    { Id: Guid
      Name: string
      Tagline: string
      Location: string
      TwitterSearch: string
      Description: string
      StartDate: DateTime
      EndDate: DateTime }

type UpdateConferenceCommand = ConferenceIdentifier * UnvalidatedConferenceInfo

// success output

type ConferenceUpdated = Conference
type UpdateConferenceEvent =
    | ConferenceUpdated of ConferenceUpdated

// error output
type ValidationError = ValidationError of string
type UpdateConferenceError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.NotFound

// workflow
type UpdateConference =
    UpdateConferenceCommand -> AsyncResult<UpdateConferenceEvent list, UpdateConferenceError>


namespace eShop.Domain.ConferenceManagement.UpdateConference

open System
open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

// input
type UnvalidatedConferenceInfo =
    { Id: Guid
      Name: string
      Tagline: string
      Location: string
      TwitterSearch: string
      Description: string
      StartDate: DateTime
      EndDate: DateTime
      Slug: string
      AccessCode: string }

type UpdateConferenceCommand = Command<UnvalidatedConferenceInfo>

// success output
type ValidatedConferenceInfo =
    { Id: ConferenceId
      Name: String250
      Description: NotEmptyString
      Location: String250
      Tagline: String250 option
      TwitterSearch: String250 option
      StartAndEnd: StartAndEnd
      Slug: UniqueSlug
      AccessCode: AccessCode }
type ConferenceUpdated = Conference
type UpdateConferenceEvent =
    | ConferenceUpdated of ConferenceUpdated

// error output
type ValidationError = ValidationError of string
type UpdateConferenceError =
    | Validation of ValidationError

// workflow
type UpdateConference =
    UpdateConferenceCommand -> AsyncResult<UpdateConferenceEvent list, UpdateConferenceError>

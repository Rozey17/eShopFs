namespace eShop.Domain.ConferenceManagement.CreateConference

open System
open eShop.Infrastructure
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Common

// input
type UnvalidatedConferenceInfo =
    { OwnerName: string
      OwnerEmail: string
      Slug: string
      Name: string
      Tagline: string
      Location: string
      TwitterSearch: string
      Description: string
      StartDate: DateTime
      EndDate: DateTime }

type CreateConferenceCommand = Command<UnvalidatedConferenceInfo>

// success output
type ConferenceCreated = Conference
type CreateConferenceEvent =
    | ConferenceCreated of ConferenceCreated

// error output
type ValidationError = ValidationError of string
type CreateConferenceError =
    | Validation of ValidationError

// workflow
type CreateConference =
    CreateConferenceCommand -> AsyncResult<CreateConferenceEvent list, CreateConferenceError>

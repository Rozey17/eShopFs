namespace eShop.Domain.ConferenceManagement.CreateConference

open System
open eShop.Infrastructure.FSharp
open eShop.Infrastructure.Db
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Common

// input
type UnvalidatedConference =
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

type CreateConferenceCommand = Command<UnvalidatedConference>

// success output (event list)
// here we only have ONE event
type ConferenceCreated = ConferenceCreated of Conference

// error output
type ValidationError = ValidationError of string

type CreateConferenceError =
    | Validation of ValidationError
    | Database of DbError

// workflow
type CreateConference =
    CreateConferenceCommand -> AsyncResult<ConferenceCreated, CreateConferenceError>

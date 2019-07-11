namespace eShop.Domain.Conference.ReadModel.ReadConferenceDetails

open System
open eShop.Infrastructure

// input
type ConferenceIdentifier = string * string // slug * accessCode

// output
[<CLIMutable>]
type ConferenceDetailsDTO =
    { Id: Guid
      Name: string
      Description: string
      Location: string
      Tagline: string
      Slug: string
      TwitterSearch: string
      StartDate: DateTime
      EndDate: DateTime
      AccessCode: string
      OwnerName: string
      OwnerEmail: string
      WasEverPublished: bool
      IsPublished: bool }

// error
type RecordNotFound = RecordNotFound

// query
type ReadConferenceDetails = ConferenceIdentifier -> AsyncResult<ConferenceDetailsDTO, RecordNotFound>

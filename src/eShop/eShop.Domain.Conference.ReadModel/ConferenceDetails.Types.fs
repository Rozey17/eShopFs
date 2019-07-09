namespace eShop.Domain.Conference.ReadModel

open System
open eShop.Infrastructure

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

type RecordNotFound = RecordNotFound

type ReadConferenceDetails = string * string -> AsyncResult<ConferenceDetailsDTO, RecordNotFound>

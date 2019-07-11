namespace eShop.Domain.Registration.Conference.ReadModel.ReadConferenceDetails

open System
open eShop.Infrastructure

type Slug = string

[<CLIMutable>]
type ConferenceDetailsDTO =
    { Id: Guid
      Slug: string
      Name: string
      Description: string
      Location: string
      Tagline: string
      TwitterSearch: string
      StartDate: DateTime }

type RecordNotFound = RecordNotFound

type ReadConferenceDetails = Slug -> AsyncResult<ConferenceDetailsDTO, RecordNotFound>

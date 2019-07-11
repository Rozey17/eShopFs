namespace eShop.Domain.Registration.Conference.ReadModel.ReadPublishedConferences

open System

[<CLIMutable>]
type ConferenceEntryDTO =
    { Id: Guid
      Slug: string
      Name: string
      Tagline: string }

type ReadPublishedConferences = unit -> Async<ConferenceEntryDTO seq>

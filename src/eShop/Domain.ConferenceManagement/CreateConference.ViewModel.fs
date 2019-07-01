module eShop.Domain.ConferenceManagement.CreateConference.ViewModel

open System

[<CLIMutable>]
type CreateConferenceViewModel =
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


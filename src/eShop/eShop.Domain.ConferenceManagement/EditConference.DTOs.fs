namespace eShop.Domain.ConferenceManagement.EditConference

open System

[<CLIMutable>]
type EditConferenceFormDTO =
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

namespace eShop.Domain.ConferenceManagement.CreateConference

open System

[<CLIMutable>]
type ConferenceFormDto =
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

module internal ConferenceFormDto =

    /// Used when importing an Conference Form from outside world into the domain
    let toUnvalidatedConferenceInfo (dto: ConferenceFormDto) =
        let domainObj: UnvalidatedConference =
            { OwnerName = dto.OwnerName
              OwnerEmail = dto.OwnerEmail
              Slug = dto.Slug
              Name = dto.Name
              Tagline = dto.Tagline
              Location = dto.Location
              TwitterSearch = dto.TwitterSearch
              Description = dto.Description
              StartDate = dto.StartDate
              EndDate = dto.EndDate }
        domainObj

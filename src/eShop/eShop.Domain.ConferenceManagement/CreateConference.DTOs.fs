namespace eShop.Domain.ConferenceManagement.CreateConference

open System

[<CLIMutable>]
type ConferenceFormDTO =
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

module ConferenceFormDTO =

    /// Used when importing an Conference Form from outside world into the domain
    let toUnvalidatedConferenceInfo (dto: ConferenceFormDTO) =
        let domainObj: UnvalidatedConferenceInfo =
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

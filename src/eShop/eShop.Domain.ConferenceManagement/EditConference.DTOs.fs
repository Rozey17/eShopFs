namespace eShop.Domain.ConferenceManagement.EditConference

open System
open eShop.Domain.ConferenceManagement.ConferenceDetails

[<CLIMutable>]
type ConferenceFormDTO =
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
      IsPublished: bool }

module ConferenceFormDTO =

    let fromConferenceDetailsDTO (dto: ConferenceDetailsDTO) =
        let formDTO: ConferenceFormDTO =
            { Id = dto.Id
              Name = dto.Name
              Description = dto.Description
              Location = dto.Location
              Tagline = dto.Tagline
              Slug = dto.Slug
              TwitterSearch = dto.TwitterSearch
              StartDate = dto.StartDate
              EndDate = dto.EndDate
              AccessCode = dto.AccessCode
              OwnerName = dto.OwnerName
              OwnerEmail = dto.OwnerEmail
              IsPublished = dto.IsPublished }
        formDTO

    let toUnvalidatedConferenceInfo (dto: ConferenceFormDTO) =
        let domainObj: UnvalidatedConferenceInfo =
            { Id = dto.Id
              Name = dto.Name
              Tagline = dto.Tagline
              TwitterSearch = dto.TwitterSearch
              Location = dto.Location
              Description = dto.Description
              StartDate = dto.StartDate
              EndDate = dto.EndDate }
        domainObj

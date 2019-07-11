namespace eShop.Domain.Conference.Web.EditConference

open System
open eShop.Domain.Conference.UpdateConference
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails

// dto
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

module EditConferenceFormDTO =

    let fromConferenceDetailsDTO (details: ConferenceDetailsDTO) =
        let form: EditConferenceFormDTO =
            { Id = details.Id
              Name = details.Name
              Description = details.Description
              Location = details.Location
              Tagline = details.Tagline
              Slug = details.Slug
              TwitterSearch = details.TwitterSearch
              StartDate = details.StartDate
              EndDate = details.EndDate
              AccessCode = details.AccessCode
              OwnerName = details.OwnerName
              OwnerEmail = details.OwnerEmail
              WasEverPublished = details.WasEverPublished
              IsPublished = details.IsPublished }
        form

    /// Used when importing an Conference Form from outside world into the domain
    let toUnvalidatedConferenceInfo (dto: EditConferenceFormDTO) =
        let domainObj: UnvalidatedConferenceInfo =
            { Id = dto.Id
              Name = dto.Name
              Tagline = dto.Tagline
              Location = dto.Location
              TwitterSearch = dto.TwitterSearch
              Description = dto.Description
              StartDate = dto.StartDate
              EndDate = dto.EndDate }
        domainObj


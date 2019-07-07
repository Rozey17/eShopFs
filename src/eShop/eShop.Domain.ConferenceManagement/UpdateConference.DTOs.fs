namespace eShop.Domain.ConferenceManagement.UpdateConference

open eShop.Domain.ConferenceManagement.EditConference

module EditConferenceFormDTO =

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
              EndDate = dto.EndDate
              Slug = dto.Slug
              AccessCode = dto.AccessCode }
        domainObj

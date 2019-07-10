namespace eShop.Domain.Conference.Web.CreateConference

open System
open eShop.Domain.Common
open eShop.Domain.Conference
open eShop.Domain.Conference.CreateConference

// internal dto
[<CLIMutable>]
type ConferenceCreatedDTO =
    { Id: Guid
      Name: string
      Slug: string
      Description: string
      Location: string
      Tagline: string
      TwitterSearch: string
      StartDate: DateTime
      IsPublished: bool }

module ConferenceCreatedDTO =

    let fromDomain (e: ConferenceCreated) =
        let info = e |> Conference.info

        { Id = info.Id |> ConferenceId.value
          Name = info.Name |> String250.value
          Slug = info.Slug |> NotEditableUniqueSlug.value
          Description = info.Description |> NotEmptyString.value
          Location = info.Location |> String250.value
          Tagline = info.Tagline |> Option.map String250.value |> Option.defaultValue null
          TwitterSearch = info.TwitterSearch |> Option.map String250.value |> Option.defaultValue null
          StartDate = info.StartAndEnd |> StartAndEnd.startDateValue
          IsPublished = false }

// web dto
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
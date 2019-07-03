namespace eShop.Domain.ConferenceManagement.Database

open System
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Common

type ConferenceDTO =
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
      CanDeleteSeat: bool }

module ConferenceDTO =

    let fromDomain (Conference(info, canDeleteSeat)) =
        { Id = info.Id |> ConferenceId.value
          Name = info.Name |> String250.value
          Description = info.Description |> NotEmptyString.value
          Location = info.Location |> String250.value
          Tagline = info.Tagline |> Option.map String250.value |> Option.defaultValue null
          Slug = info.Slug |> NotEditableUniqueSlug.value
          TwitterSearch = info.TwitterSearch |> Option.map String250.value |> Option.defaultValue null
          StartDate = info.StartDate |> Date.value
          EndDate = info.EndDate |> Date.value
          AccessCode = info.AccessCode |> GeneratedAndNotEditableAccessCode.value
          OwnerName = info.Owner.Name |> String250.value
          OwnerEmail = info.Owner.Email |> EmailAddress.value
          CanDeleteSeat = canDeleteSeat }

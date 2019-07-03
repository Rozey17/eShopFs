module internal eShop.Domain.ConferenceManagement.CreateConference.Database

open System
open eShop.Infrastructure.Db
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
      CanDeleteSeat: bool
      IsPublished: bool }

module ConferenceDTO =

    let fromDomain (conference: Conference) =
        let info, publised, canDeleteSeat =
            match conference with
            | PublisedConference info -> info, true, false
            | UnpublishedConference (info, canDeleteSeat) -> info, false, canDeleteSeat

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
          CanDeleteSeat = canDeleteSeat
          IsPublished = publised }

let insertConferenceIntoDb connection conference =
    let sql = @"
        insert into
            conference
            (
                id,
                name,
                description,
                location,
                tagline,
                slug,
                twitter_search,
                start_date,
                end_date,
                access_code,
                owner_name,
                owner_email,
                can_delete_seat,
                is_published
            )
            values
            (
                @Id,
                @Name,
                @Description,
                @Location,
                @Tagline,
                @Slug,
                @TwitterSearch,
                @StartDate,
                @EndDate,
                @AccessCode,
                @OwnerName,
                @OwnerEmail,
                @CanDeleteSeat,
                @IsPublished
            )"
    let dto = ConferenceDTO.fromDomain conference
    Dapper.parametrizedExecuteAsync connection sql dto

let checkSlugExists connection (slug: NotEditable<UniqueSlug>) =
    let sql = @"
        select exists
               (
                   select 1
                     from conference
                    where slug = @Slug
               )"
    let slug = slug |> NotEditableUniqueSlug.value
    Dapper.mapParametrizedQuerySingleAsync<bool> connection sql (Map ["Slug", slug])

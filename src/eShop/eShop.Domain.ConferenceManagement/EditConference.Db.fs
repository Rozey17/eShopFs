module eShop.Domain.ConferenceManagement.EditConference.Db

open System
open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

module ReadConferenceDetails =

    [<CLIMutable>]
    type ConferenceDetailsDbDTO =
        { id: Guid
          name: string
          description: string
          location: string
          tagline: string
          slug: string
          twitter_search: string
          start_date: DateTime
          end_date: DateTime
          access_code: string
          owner_name: string
          owner_email: string
          can_delete_seat: bool
          is_published: bool }

    let query connection slug accessCode =
        let slug = slug |> UniqueSlug.value
        let accessCode = accessCode |> AccessCode.value

        let sql = @"
            select id,
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
              from conference
             where slug = @Slug
               and access_code = @AccessCode"
        let param = {| Slug = slug; AccessCode = accessCode |}

        async {
            let! result = Db.tryParameterizedQuerySingleAsync<ConferenceDetailsDbDTO> connection sql param
            match result with
            | Some record ->
                let dto =
                    { Id = record.id
                      Name = record.name
                      Description = record.description
                      Location = record.location
                      Tagline = record.tagline
                      Slug = record.slug
                      TwitterSearch = record.twitter_search
                      StartDate = record.start_date
                      EndDate = record.end_date
                      AccessCode = record.access_code
                      OwnerName = record.owner_name
                      OwnerEmail = record.owner_email
                      CanDeleteSeat = record.can_delete_seat
                      IsPublished = record.is_published }
                return Some dto

            | None ->
                return None
        }

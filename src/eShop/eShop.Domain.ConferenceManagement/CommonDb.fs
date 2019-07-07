namespace eShop.Domain.ConferenceManagement.Db

open System
open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

[<RequireQualifiedAccess>]
module CommonDb =
    let exnOnError v =
        match v with
        | Ok r -> r
        | Error _ -> failwith "db is in invalid state"

    module ReadSingleConference =

        [<CLIMutable>]
        type ConferenceDbDTO =
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

        let execute connection id =
            let id = id |> ConferenceId.value
            let sql =
                """
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
                 where id = @Id
                """
            let param = {| Id = id |}

            async {
                let! dto = Db.parameterizedQuerySingleAsync<ConferenceDbDTO> connection sql param
                let info =
                    { Id = dto.id |> ConferenceId.create |> exnOnError
                      Name = dto.name |> String250.create "Name" |> exnOnError
                      Description = dto.description |> NotEmptyString.create "Description" |> exnOnError
                      Location = dto.location |> String250.create "Location" |> exnOnError
                      Tagline = dto.tagline |> String250.createOption "Tagline" |> exnOnError
                      Slug = dto.slug |> UniqueSlug.create |> exnOnError |> NotEditable
                      TwitterSearch = dto.twitter_search |> String250.createOption "Twitter Search" |> exnOnError
                      StartAndEnd = (dto.start_date, dto.end_date) |> StartAndEnd.create |> exnOnError
                      AccessCode = dto.access_code |> AccessCode.create |> exnOnError |> Generated |> NotEditable
                      Owner =
                          { Name = dto.owner_name |> String250.create "Owner Name" |> exnOnError
                            Email = dto.owner_email |> EmailAddress.create "Owner Email" |> exnOnError } |> NotEditable }
                match dto.is_published with
                | true ->
                    return PublisedConference info
                | false ->
                    return UnpublishedConference (info = info, canDeleteSeat = dto.can_delete_seat)
            }

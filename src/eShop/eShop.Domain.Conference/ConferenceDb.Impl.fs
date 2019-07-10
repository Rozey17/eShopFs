module eShop.Domain.Conference.ConferenceDb.Impl

open System
open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.Conference

let private exnOnError v =
    match v with
    | Ok r -> r
    | Error _ -> failwith "db is in invalid state"

module ReadSingleConference =

    [<CLIMutable>]
    type ConferenceDTO =
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
          was_ever_published: bool
          is_published: bool }

    module ConferenceDTO =
        let toConferenceInfo (dto: ConferenceDTO) =
            let domainObj =
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
            domainObj

    let query connection : ReadSingleConference =
        fun (slug, accessCode) ->
            let slug = slug |> UniqueSlug.value
            let accessCode = accessCode |> AccessCode.value
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
                       was_ever_published,
                       is_published
                  from cm.conference
                 where slug = @Slug
                   and access_code = @AccessCode
                """
            let param = {| Slug = slug; AccessCode = accessCode |}

            async {
                let! queryResult = Db.tryParameterizedQuerySingleAsync<ConferenceDTO> connection sql param
                match queryResult with
                | Some dto ->
                    let info = dto |> ConferenceDTO.toConferenceInfo
                    if dto.is_published then
                        return Ok (PublishedConference info)
                    else
                        return Ok (UnpublishedConference (info, dto.was_ever_published))
                | None ->
                    return Error (RecordNotFound)
            }

module CheckSlugExists =

    let execute connection : CheckSlugExists =
        fun slug ->
            let slug = slug |> UniqueSlug.value
            let sql = @"
                select exists
                       (
                           select 1
                             from cm.conference
                            where slug = @Slug
                       )"
            let param = {| Slug = slug |}
            Db.parameterizedQuerySingleAsync<bool> connection sql param


module InsertConference =

    module ConferenceInfoDTO =
        let fromDomain info =
            {| Id = info.Id |> ConferenceId.value
               Name = info.Name |> String250.value
               Description = info.Description |> NotEmptyString.value
               Location = info.Location |> String250.value
               Tagline = info.Tagline |> Option.map String250.value |> Option.defaultValue null
               Slug = info.Slug |> NotEditableUniqueSlug.value
               TwitterSearch = info.TwitterSearch |> Option.map String250.value |> Option.defaultValue null
               StartDate = info.StartAndEnd |> StartAndEnd.startDateValue
               EndDate = info.StartAndEnd |> StartAndEnd.endDateValue
               AccessCode = info.AccessCode |> GeneratedAndNotEditableAccessCode.value
               OwnerName = info.Owner |> NotEditableOwnerInfo.name
               OwnerEmail = info.Owner |> NotEditableOwnerInfo.email
               WasEverPublished = false
               IsPublished = false |}

    let execute connection : InsertConference =
        fun conference ->
            match conference with
            | UnpublishedConference (info, wasEverPublish) when wasEverPublish = false ->
                let sql =
                    """
                    insert into
                        cm.conference
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
                            was_ever_published,
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
                            @WasEverPublished,
                            @IsPublished
                        )
                    """
                let dto = ConferenceInfoDTO.fromDomain info
                Db.parameterizedExecuteAsync connection sql dto
            | _ ->
                async.Zero()


module UpdateConference =

    module ConferenceDTO =

        let fromDomain conference =
            let info = conference |> Conference.info

            {| Id = info.Id |> ConferenceId.value
               Name = info.Name |> String250.value
               Description = info.Description |> NotEmptyString.value
               Location = info.Location |> String250.value
               Tagline = info.Tagline |> Option.map String250.value |> Option.defaultValue null
               TwitterSearch = info.TwitterSearch |> Option.map String250.value |> Option.defaultValue null
               StartDate = info.StartAndEnd |> StartAndEnd.startDateValue
               EndDate = info.StartAndEnd |> StartAndEnd.endDateValue |}

    let execute connection : UpdateConference =
        fun conference ->
            let sql =
                """
                update cm.conference
                   set name = @Name,
                       description = @Description,
                       location = @Location,
                       tagline = @Tagline,
                       twitter_search = @TwitterSearch,
                       start_date = @StartDate,
                       end_date = @EndDate
                 where id = @Id
                """
            let dto = ConferenceDTO.fromDomain conference
            Db.parameterizedExecuteAsync connection sql dto


module MarkConferenceAsPublished =

    let execute connection : MarkConferenceAsPublished =
        fun conference ->
            match conference with
            | PublishedConference info ->
                let id = info.Id |> ConferenceId.value
                let sql =
                    """
                    update cm.conference
                       set is_published = 't',
                           was_ever_published = 't'
                     where id = @Id
                    """
                let param = {| Id = id |}
                Db.parameterizedExecuteAsync connection sql param
            | _ ->
                async.Zero()


module MarkConferenceAsUnpublished =
    let execute connection : MarkConferenceAsUnpublished =
        fun conference ->
            match conference with
            | UnpublishedConference (info, _) ->
                let id = info.Id |> ConferenceId.value
                let sql =
                    """
                    update cm.conference
                       set is_published = 'f'
                     where id = @Id
                    """
                let param = {| Id = id |}
                Db.parameterizedExecuteAsync connection sql param
            | _ ->
                async.Zero()

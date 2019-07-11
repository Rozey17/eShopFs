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
    type ConferenceInfoDTO =
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
        let toConferenceInfo dto =
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

    [<CLIMutable>]
    type SeatTypeDTO =
        { conference_id: Guid
          id: Guid
          name: string
          description: string
          quantity: int
          price: decimal }
    module SeatTypeDTO =
        let toSeatType dto =
            let domainObj =
                { ConferenceId = dto.conference_id |> ConferenceId.create |> exnOnError
                  Id = dto.id |> SeatTypeId.create |> exnOnError
                  Name = dto.name |> Name.create |> exnOnError
                  Description = dto.description |> String250.create "Description" |> exnOnError
                  Quantity = dto.quantity |> UnitQuantity.create "Quantity" |> exnOnError
                  Price = dto.price |> Price.create "Price" |> exnOnError }
            domainObj

    let query connection : ReadSingleConference =
        fun id ->
            let id = id |> ConferenceId.value
            let sql1 =
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
                 where id = @Id
                """
            let sql2 =
                """
                select conference_id,
                       id,
                       name,
                       description,
                       quantity,
                       price
                  from cm.seat
                 where conference_id = @ConferenceId
                """

            async {
                let param1 = {| Id = id |}
                let! infoResult = Db.tryParameterizedQuerySingleAsync<ConferenceInfoDTO> connection sql1 param1

                match infoResult with
                | Some infoDTO ->
                    let info = infoDTO |> ConferenceDTO.toConferenceInfo

                    let param2 = {| ConferenceId = infoDTO.id |}
                    let! seatDTOs = Db.parameterizedQueryAsync<SeatTypeDTO> connection sql2 param2

                    let seats =
                        seatDTOs
                        |> Seq.map SeatTypeDTO.toSeatType
                        |> List.ofSeq

                    if infoDTO.is_published then
                        return Ok (PublishedConference (info, seats))
                    else
                        return Ok (UnpublishedConference (info, infoDTO.was_ever_published, seats))
                | None ->
                    return Error (RecordNotFound)
            }

module CheckSlugExists =
    let execute connection : CheckSlugExists =
        fun slug ->
            let slug = slug |> UniqueSlug.value
            let sql =
                """
                select exists
                       (
                           select 1
                             from cm.conference
                            where slug = @Slug
                       )
                """
            let param = {| Slug = slug |}
            Db.parameterizedQuerySingleAsync<bool> connection sql param


module InsertConference =

    module ConferenceInfoDTO =
        let fromDomain (info: ConferenceInfo) =
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
            let info = conference |> Conference.info
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
            | PublishedConference (info, _) ->
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
            | UnpublishedConference (info, _, _) ->
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


module InsertSeatType =

    [<CLIMutable>]
    type SeatTypeDTO =
        { ConferenceId: Guid
          Id: Guid
          Name: string
          Description: string
          Quantity: int
          Price: decimal }
    module SeatTypeDTO =
        let fromDomain (seatType: SeatType) =
            { ConferenceId = seatType.ConferenceId |> ConferenceId.value
              Id = seatType.Id |> SeatTypeId.value
              Name = seatType.Name |> Name.value
              Description = seatType.Description |> String250.value
              Quantity = seatType.Quantity |> UnitQuantity.value
              Price = seatType.Price |> Price.value }

    let execute connection : InsertSeatType =
        fun (_conference, seatType) ->
            let sql =
                """
                insert into
                    cm.seat
                    (
                        conference_id,
                        id,
                        name,
                        description,
                        quantity,
                        price
                    )
                    values
                    (
                        @ConferenceId,
                        @Id,
                        @Name,
                        @Description,
                        @Quantity,
                        @Price
                    )
                """
            let dto = SeatTypeDTO.fromDomain seatType
            Db.parameterizedExecuteAsync connection sql dto

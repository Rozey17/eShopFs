module eShop.Domain.ConferenceManagement.CreateConference.Db

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

module InsertConferenceIntoDb =

    module ConferenceDbDTO =

        let fromDomain (UnpublishedConference (info, _)) =
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

    let execute connection conference =
        let sql =
            """
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
        let dto = ConferenceDbDTO.fromDomain conference
        Db.parameterizedExecuteAsync connection sql dto

module CheckSlugExists =

    let query connection (slug: UniqueSlug) =
        let sql = @"
            select exists
                   (
                       select 1
                         from conference
                        where slug = @Slug
                   )"
        let slug = slug |> UniqueSlug.value
        let param = {| Slug = slug |}
        Db.parameterizedQuerySingleAsync<bool> connection sql param

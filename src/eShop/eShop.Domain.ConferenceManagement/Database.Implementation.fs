module eShop.Domain.ConferenceManagement.Database.DatabaseImplementation

open eShop.Infrastructure.Db
open eShop.Domain.ConferenceManagement.Common

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
                can_delete_seat
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
                @CanDeleteSeat
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
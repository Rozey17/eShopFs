module eShop.Domain.ConferenceManagement.ConferenceDetails.Db

open System
open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

type ReadParam =
    { Slug: string
      AccessCode: string }

type ReadResult =
    { name: string
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
      is_published: bool }

let readConferenceDetails connection slug accessCode =
    let slug = slug |> NotEditableUniqueSlug.value
    let accessCode = accessCode |> GeneratedAndNotEditableAccessCode.value

    let sql = @"
        select name,
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
               is_published
          from conference
         where slug = @Slug
           and access_code = @AccessCode"
    let param = { Slug = slug; AccessCode = accessCode }

    async {
        let! result = Db.tryParameterizedQuerySingleAsync<ReadResult> connection sql param
        match result with
        | None ->
            return None
        | Some record ->
            let dto =
                { Name = record.name
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
                  IsPublished = record.is_published }

            return Some dto
    }



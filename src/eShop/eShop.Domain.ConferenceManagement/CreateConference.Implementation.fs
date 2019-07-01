module internal eShop.Domain.ConferenceManagement.CreateConference.Implementation

open eShop.Infrastructure.FSharp
open eShop.Infrastructure.Db
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Common

// -----
// types
// -----

// validate
type ValidatedConferenceInfo =
    { Name: String250
      Description: NotEmptyString
      Location: String250
      Tagline: String250 option
      Slug: UniqueSlug
      TwitterSearch: String250 option
      StartDate: Date
      EndDate: Date
      Owner: OwnerInfo }

type CheckSlugExists = UniqueSlug -> Async<bool>

type ValidateConferenceInfo =
    CheckSlugExists                                            // dependency
     -> UnvalidatedConferenceInfo                              // input
     -> AsyncResult<ValidatedConferenceInfo, ValidationError>  // output

// -----
// impl
// -----
let validateConferenceInfo: ValidateConferenceInfo =
    fun checkSlugExists conference ->
        asyncResult {
            let! ownerName =
                conference.OwnerName
                |> String250.create "OwnerName"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! ownerEmail =
                conference.OwnerEmail
                |> EmailAddress.create "OwnerEmail"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! name =
                conference.Name
                |> String250.create "Name"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! description =
                conference.Description
                |> NotEmptyString.create "Description"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! location =
                conference.Location
                |> String250.create "Location"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! tagline =
                conference.Tagline
                |> String250.createOption "Tagline"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! slug =
                conference.Slug
                |> UniqueSlug.create "Slug"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! slugExisted =
                slug
                |> checkSlugExists
                |> AsyncResult.ofAsync
            do! if slugExisted then
                    Error (ValidationError "Slug is already taken.")
                else
                    Ok ()
                |> AsyncResult.ofResult
            let! twitterSearch =
                conference.TwitterSearch
                |> String250.createOption "TwitterSearch"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! startDate =
                conference.StartDate
                |> Date.create "StartDate"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! endDate =
                conference.EndDate
                |> Date.create "EndDate"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            do! if startDate > endDate then
                    Error (ValidationError "StartDate can not come after EndDate")
                else
                    Ok ()
                |> AsyncResult.ofResult

            let validatedConferenceInfo: ValidatedConferenceInfo =
                { Name = name
                  Description = description
                  Location = location
                  Tagline = tagline
                  Slug = slug
                  TwitterSearch = twitterSearch
                  StartDate = startDate
                  EndDate = endDate
                  Owner =
                      { Name = ownerName
                        Email = ownerEmail } }

            return validatedConferenceInfo
        }

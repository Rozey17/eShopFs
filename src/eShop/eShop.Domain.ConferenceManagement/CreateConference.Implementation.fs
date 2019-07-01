module internal eShop.Domain.ConferenceManagement.CreateConference.Implementation

open eShop.Infrastructure.FSharp
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Common

// types
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
type ValidateConferenceInfo =
    UnvalidatedConference -> Result<ValidatedConferenceInfo, ValidationError>

// impl
let validateConferenceInfo: ValidateConferenceInfo =
    fun conference ->
        result {
            let! ownerName =
                conference.OwnerName
                |> String250.create "OwnerName"
                |> Result.mapError ValidationError
            let! ownerEmail =
                conference.OwnerEmail
                |> EmailAddress.create "OwnerEmail"
                |> Result.mapError ValidationError
            let! name =
                conference.Name
                |> String250.create "Name"
                |> Result.mapError ValidationError
            let! description =
                conference.Description
                |> NotEmptyString.create "Description"
                |> Result.mapError ValidationError
            let! location =
                conference.Location
                |> String250.create "Location"
                |> Result.mapError ValidationError
            let! tagline =
                conference.Tagline
                |> String250.createOption "Tagline"
                |> Result.mapError ValidationError
            // TODO: query db to check unique
            let! slug =
                conference.Slug
                |> UniqueSlug.create "Slug"
                |> Result.mapError ValidationError
            let! twitterSearch =
                conference.TwitterSearch
                |> String250.createOption "TwitterSearch"
                |> Result.mapError ValidationError
            let! startDate =
                conference.StartDate
                |> Date.create "StartDate"
                |> Result.mapError ValidationError
            let! endDate =
                conference.EndDate
                |> Date.create "EndDate"
                |> Result.mapError ValidationError
            do!
                if startDate > endDate then
                    Error (ValidationError "StartDate can not come after EndDate")
                else
                    Ok ()

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
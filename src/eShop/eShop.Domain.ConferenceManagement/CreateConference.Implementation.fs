module internal eShop.Domain.ConferenceManagement.CreateConference.Implementation

open eShop.Infrastructure.FSharp
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
      Slug: NotEditable<UniqueSlug>
      TwitterSearch: String250 option
      StartDate: Date
      EndDate: Date
      Owner: OwnerInfo }

module ValidatedConferenceInfo =

    let toConferenceInfoWith id accessCode validatedInfo =
        { Id = id
          AccessCode = accessCode
          Name = validatedInfo.Name
          Description = validatedInfo.Description
          Location = validatedInfo.Location
          Tagline = validatedInfo.Tagline
          Slug = validatedInfo.Slug
          TwitterSearch = validatedInfo.TwitterSearch
          StartDate = validatedInfo.StartDate
          EndDate = validatedInfo.EndDate
          Owner = validatedInfo.Owner }

type CheckSlugExists = NotEditable<UniqueSlug> -> Async<bool>

type ValidateConferenceInfo =
    CheckSlugExists                                            // dependency
     -> UnvalidatedConferenceInfo                              // input
     -> AsyncResult<ValidatedConferenceInfo, ValidationError>  // output

// insert
type InsertConferenceIntoDb = Conference -> Async<unit>

// create events
type CreateEvents = Conference -> CreateConferenceEvent list

// -----
// impl
// -----
let validateSlugExists (checkSlugExists: CheckSlugExists) slug =
    async {
        let! existed = checkSlugExists slug
        if existed then
            return Error "Slug is already taken"
        else
            return Ok slug
    }

let validateDateOrder (startDate: Date) (endDate: Date) =
    if startDate > endDate then
        Error "StartDate can not come after EndDate"
    else
        Ok (startDate, endDate)

let validateConferenceInfo: ValidateConferenceInfo =
    fun checkSlugExists unvalidatedInfo ->
        asyncResult {
            let! ownerName =
                unvalidatedInfo.OwnerName
                |> String250.create "OwnerName"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! ownerEmail =
                unvalidatedInfo.OwnerEmail
                |> EmailAddress.create "OwnerEmail"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! name =
                unvalidatedInfo.Name
                |> String250.create "Name"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! description =
                unvalidatedInfo.Description
                |> NotEmptyString.create "Description"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! location =
                unvalidatedInfo.Location
                |> String250.create "Location"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! tagline =
                unvalidatedInfo.Tagline
                |> String250.createOption "Tagline"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! slug =
                unvalidatedInfo.Slug
                |> NotEditableUniqueSlug.create "Slug"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            do! slug
                |> validateSlugExists checkSlugExists
                |> AsyncResult.mapError ValidationError
                |> AsyncResult.ignore
            let! twitterSearch =
                unvalidatedInfo.TwitterSearch
                |> String250.createOption "TwitterSearch"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! startDate =
                unvalidatedInfo.StartDate
                |> Date.create "StartDate"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! endDate =
                unvalidatedInfo.EndDate
                |> Date.create "EndDate"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            do! (startDate, endDate)
                ||> validateDateOrder
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
                |> AsyncResult.ignore

            let validatedInfo: ValidatedConferenceInfo =
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

            return validatedInfo
        }

let createConferenceCreatedEvent (conference: Conference) : ConferenceCreated = conference

let createEvents: CreateEvents =
    fun conference ->
        let conferenceCreated =
            conference
            |> createConferenceCreatedEvent
            |> CreateConferenceEvent.ConferenceCreated

        [
            yield conferenceCreated
        ]

let createConference
    (checkSlugExists: CheckSlugExists)                 // dependency
    (insertConferenceIntoDb: InsertConferenceIntoDb)   // dependency
    : CreateConference =                               // definition of function

    fun command ->                                     // input
        asyncResult {
            let! validatedInfo =
                validateConferenceInfo checkSlugExists command.Data
                |> AsyncResult.mapError CreateConferenceError.Validation

            let id = ConferenceId.generate()
            let accessCode = GeneratedAndNotEditableAccessCode.generate()
            let conferenceInfo =
                validatedInfo
                |> ValidatedConferenceInfo.toConferenceInfoWith id accessCode
            let conference = Conference(info=conferenceInfo, canDeleteSeat=true)

            do! insertConferenceIntoDb conference
                |> AsyncResult.ofAsync

            let events = conference |> createEvents
            return events
        }

module eShop.Domain.ConferenceManagement.CreateConference.Impl

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

// -----
// types
// -----

// step: validate
type ValidatedConferenceInfo =
    { Name: String250
      Description: NotEmptyString
      Location: String250
      Tagline: String250 option
      Slug: UniqueSlug
      TwitterSearch: String250 option
      StartAndEnd: StartAndEnd
      Owner: OwnerInfo }

module ValidatedConferenceInfo =

    let toConferenceInfoWith id accessCode validatedInfo =
        { Id = id
          AccessCode = accessCode |> Generated |> NotEditable
          Name = validatedInfo.Name
          Description = validatedInfo.Description
          Location = validatedInfo.Location
          Tagline = validatedInfo.Tagline
          Slug = validatedInfo.Slug |> NotEditable
          TwitterSearch = validatedInfo.TwitterSearch
          StartAndEnd = validatedInfo.StartAndEnd
          Owner = validatedInfo.Owner |> NotEditable }

type CheckSlugExists = UniqueSlug -> Async<bool>

type ValidateConferenceInfo =
    CheckSlugExists                                            // dependency
     -> UnvalidatedConferenceInfo                              // input
     -> AsyncResult<ValidatedConferenceInfo, ValidationError>  // output

// step: insert into db
type InsertConferenceIntoDb = UnpublishedConference -> Async<unit>

// step: create events
type CreateEvents = UnpublishedConference -> CreateConferenceEvent list

// -----
// impl
// -----
let validateSlugExists (checkSlugExists: CheckSlugExists) slug =
    async {
        let! existed = checkSlugExists slug
        if existed then
            return Error "Slug is already taken"
        else
            return Ok ()
    }

let validateConferenceInfo: ValidateConferenceInfo =
    fun checkSlugExists unvalidatedInfo ->
        asyncResult {
            let! ownerName =
                unvalidatedInfo.OwnerName
                |> String250.create "Owner Name"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! ownerEmail =
                unvalidatedInfo.OwnerEmail
                |> EmailAddress.create "Owner Email"
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
                |> UniqueSlug.create
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            do! slug
                |> validateSlugExists checkSlugExists
                |> AsyncResult.mapError ValidationError
            let! twitterSearch =
                unvalidatedInfo.TwitterSearch
                |> String250.createOption "Twitter Search"
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError
            let! startAndEnd =
                (unvalidatedInfo.StartDate, unvalidatedInfo.EndDate)
                |> StartAndEnd.create
                |> AsyncResult.ofResult
                |> AsyncResult.mapError ValidationError

            let validatedInfo: ValidatedConferenceInfo =
                { Name = name
                  Description = description
                  Location = location
                  Tagline = tagline
                  Slug = slug
                  TwitterSearch = twitterSearch
                  StartAndEnd = startAndEnd
                  Owner =
                      { Name = ownerName
                        Email = ownerEmail } }

            return validatedInfo
        }

let createConferenceCreatedEvent (conference: UnpublishedConference) : ConferenceCreated = conference

let createEvents: CreateEvents =
    fun conference ->
        let conferenceCreated =
            conference
            |> createConferenceCreatedEvent
            |> ConferenceCreated

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
            let accessCode = AccessCode.generate()
            let conferenceInfo =
                validatedInfo
                |> ValidatedConferenceInfo.toConferenceInfoWith id accessCode
            let conference = UnpublishedConference(info=conferenceInfo, wasEverPublished=false)

            do! insertConferenceIntoDb conference
                |> AsyncResult.ofAsync

            let events = conference |> createEvents
            return events
        }

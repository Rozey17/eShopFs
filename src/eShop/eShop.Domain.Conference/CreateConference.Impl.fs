module eShop.Domain.Conference.CreateConference.Impl

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.Conference

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

type ValidateConferenceInfo =
    ConferenceDb.CheckSlugExists                               // dependency
     -> UnvalidatedConferenceInfo                              // input
     -> AsyncResult<ValidatedConferenceInfo, ValidationError>  // output

// step: enrich
type EnrichValidatedConferenceInfoWith = ConferenceId -> AccessCode -> ValidatedConferenceInfo -> ConferenceInfo

// step: create events
type CreateEvents = Conference -> CreateConferenceEvent list

// -----
// impl
// -----

// step: validate
let validateSlugExists (checkSlugExists: ConferenceDb.CheckSlugExists) slug =
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

// step: enrich
let enrichWith: EnrichValidatedConferenceInfoWith =
    fun id accessCode info ->
        { Id = id
          AccessCode = accessCode |> Generated |> NotEditable
          Name = info.Name
          Description = info.Description
          Location = info.Location
          Tagline = info.Tagline
          Slug = info.Slug |> NotEditable
          TwitterSearch = info.TwitterSearch
          StartAndEnd = info.StartAndEnd
          Owner = info.Owner |> NotEditable }

// step: create events
let createConferenceCreatedEvent conference : ConferenceCreated = conference

let createEvents: CreateEvents =
    fun conference ->
        let conferenceCreated =
            conference
            |> createConferenceCreatedEvent
            |> CreateConference.ConferenceCreated
        [
            yield conferenceCreated
        ]

// workflow
let createConference
    (checkSlugExists: ConferenceDb.CheckSlugExists)
    (insertConference: ConferenceDb.InsertConference)
    : CreateConference =

        fun unvalidatedInfo ->
            asyncResult {
                let! validatedInfo =
                    unvalidatedInfo
                    |> validateConferenceInfo checkSlugExists
                    |> AsyncResult.mapError CreateConferenceError.Validation

                let id = ConferenceId.generate()
                let accessCode = AccessCode.generate()
                let conferenceInfo = validatedInfo |> enrichWith id accessCode
                let conference = UnpublishedConference(info=conferenceInfo, wasEverPublished=false)

                do! insertConference conference
                    |> AsyncResult.ofAsync

                let events = conference |> createEvents
                return events
            }

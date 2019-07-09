module eShop.Domain.Conference.UpdateConference.Impl


open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.Conference

// -----
// types
// -----

// step: validate
type ValidatedConferenceInfo =
    { Id: ConferenceId
      Name: String250
      Description: NotEmptyString
      Location: String250
      Tagline: String250 option
      TwitterSearch: String250 option
      StartAndEnd: StartAndEnd }

type ValidateConferenceInfo =
    UnvalidatedConferenceInfo -> Result<ValidatedConferenceInfo, ValidationError>

// step: apply update
type ApplyUpdate = ValidatedConferenceInfo -> Conference -> Conference

// step: create events
type CreateEvents = Conference -> UpdateConferenceEvent list

// -----
// impl
// -----

// step: validate
let validateConferenceInfo: ValidateConferenceInfo =
    fun unvalidatedInfo ->
        result {
            let! id =
                unvalidatedInfo.Id
                |> ConferenceId.create
                |> Result.mapError ValidationError
            let! name =
                unvalidatedInfo.Name
                |> String250.create "Name"
                |> Result.mapError ValidationError
            let! description =
                unvalidatedInfo.Description
                |> NotEmptyString.create "Description"
                |> Result.mapError ValidationError
            let! location =
                unvalidatedInfo.Location
                |> String250.create "Location"
                |> Result.mapError ValidationError
            let! tagline =
                unvalidatedInfo.Tagline
                |> String250.createOption "Tagline"
                |> Result.mapError ValidationError
            let! twitterSearch =
                unvalidatedInfo.TwitterSearch
                |> String250.createOption "Twitter Search"
                |> Result.mapError ValidationError
            let! startAndEnd =
                (unvalidatedInfo.StartDate, unvalidatedInfo.EndDate)
                |> StartAndEnd.create
                |> Result.mapError ValidationError
            let validatedInfo: ValidatedConferenceInfo =
                { Id = id
                  Name = name
                  Description = description
                  Location = location
                  Tagline = tagline
                  TwitterSearch = twitterSearch
                  StartAndEnd = startAndEnd }

            return validatedInfo
        }

// step: apply update
let applyUpdate: ApplyUpdate =
    fun validatedInfo conference ->
        let info = conference |> Conference.info
        let changedInfo =
            { info with
                Name = validatedInfo.Name
                Description = validatedInfo.Description
                Location = validatedInfo.Location
                Tagline = validatedInfo.Tagline
                TwitterSearch = validatedInfo.TwitterSearch
                StartAndEnd = validatedInfo.StartAndEnd }
        match conference with
        | UnpublishedConference (_, wasEverPublished) ->
            UnpublishedConference (changedInfo, wasEverPublished)
        | PublishedConference _ ->
            PublishedConference changedInfo

// step: create events
let createConferenceUpdatedEvent conference : ConferenceUpdated = conference

let createEvents: CreateEvents =
    fun conference ->
        let conferenceUpdated =
            conference
            |> createConferenceUpdatedEvent
            |> UpdateConference.ConferenceUpdated
        [
            yield conferenceUpdated
        ]


// workflow
let updateConference
    (readSingleConferenceFromDb: ConferenceDb.ReadSingleConference)
    (updateConferenceInDb: ConferenceDb.UpdateConference)
    : UpdateConference =

        fun ((ConferenceIdentifier(slug, accessCode)), info) ->
            asyncResult {
                let! validatedInfo =
                    validateConferenceInfo info
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError UpdateConferenceError.Validation

                let! identifier =
                    (slug, accessCode)
                    |> Validation.validateConferenceIdentifier
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError (ValidationError >> UpdateConferenceError.Validation)
                let! conference =
                    readSingleConferenceFromDb identifier
                    |> AsyncResult.mapError UpdateConference.RecordNotFound

                let conference =
                    conference
                    |> applyUpdate validatedInfo

                do! updateConferenceInDb conference
                    |> AsyncResult.ofAsync

                let events = conference |> createEvents
                return events
            }

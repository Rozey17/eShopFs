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
    fun unvalidated ->
        result {
            let! id =
                unvalidated.Id
                |> ConferenceId.create
                |> Result.mapError ValidationError
            let! name =
                unvalidated.Name
                |> String250.create "Name"
                |> Result.mapError ValidationError
            let! description =
                unvalidated.Description
                |> NotEmptyString.create "Description"
                |> Result.mapError ValidationError
            let! location =
                unvalidated.Location
                |> String250.create "Location"
                |> Result.mapError ValidationError
            let! tagline =
                unvalidated.Tagline
                |> String250.createOption "Tagline"
                |> Result.mapError ValidationError
            let! twitterSearch =
                unvalidated.TwitterSearch
                |> String250.createOption "Twitter Search"
                |> Result.mapError ValidationError
            let! startAndEnd =
                (unvalidated.StartDate, unvalidated.EndDate)
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
        | UnpublishedConference (_, wasEverPublished, seats) ->
            UnpublishedConference (changedInfo, wasEverPublished, seats)
        | PublishedConference (_, seats) ->
            PublishedConference (changedInfo, seats)

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
                    |> AsyncResult.mapError UpdateConference.ConferenceNotFound

                let conference =
                    conference
                    |> applyUpdate validatedInfo

                do! updateConferenceInDb conference
                    |> AsyncResult.ofAsync

                let events = conference |> createEvents
                return events
            }

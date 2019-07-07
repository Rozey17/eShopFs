module eShop.Domain.ConferenceManagement.UpdateConference.Impl

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

// -----
// types
// -----

// step: read single conference
type ReadSingleConference = UniqueSlug * AccessCode -> Async<Conference>

// step: validate
type ValidateConferenceInfo =
    UnvalidatedConferenceInfo -> Result<ValidatedConferenceInfo, ValidationError>

// step: update in db
type UpdateConferenceInfoInDb = Conference -> Async<unit>

// step: create events
type CreateEvents = Conference -> UpdateConferenceEvent list

// -----
// impl
// -----

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
            let! slug =
                unvalidatedInfo.Slug
                |> UniqueSlug.create
                |> Result.mapError ValidationError
            let! accessCode =
                unvalidatedInfo.AccessCode
                |> AccessCode.create
                |> Result.mapError ValidationError
            let validatedInfo: ValidatedConferenceInfo =
                { Id = id
                  Name = name
                  Description = description
                  Location = location
                  Tagline = tagline
                  TwitterSearch = twitterSearch
                  StartAndEnd = startAndEnd
                  Slug = slug
                  AccessCode = accessCode }

            return validatedInfo
        }

let applyUpdateConference (validatedInfo: ValidatedConferenceInfo) conference =
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
    | Unpublished (UnpublishedConference (_, wasEverPublished)) ->
        Unpublished (UnpublishedConference (changedInfo, wasEverPublished))
    | Published _ ->
        Published (PublishedConference changedInfo)

let createConferenceUpdatedEvent conference : ConferenceUpdated = conference

let createEvents: CreateEvents =
    fun validatedInfo ->
        let conferenceUpdated =
            validatedInfo
            |> createConferenceUpdatedEvent
            |> UpdateConferenceEvent.ConferenceUpdated

        [
            yield conferenceUpdated
        ]

let updateConference
    (readSingleConference: ReadSingleConference)
    (updateConferenceInfoInDb: UpdateConferenceInfoInDb)
    : UpdateConference =

    fun cmd ->
        asyncResult {
            let! validatedInfo =
                validateConferenceInfo cmd.Data
                |> AsyncResult.ofResult
                |> AsyncResult.mapError UpdateConferenceError.Validation
            let! conference =
                readSingleConference (validatedInfo.Slug, validatedInfo.AccessCode)
                |> AsyncResult.ofAsync
            let conference = conference |> applyUpdateConference validatedInfo
            do! updateConferenceInfoInDb conference
                |> AsyncResult.ofAsync
            let events = conference |> createEvents

            return events
        }

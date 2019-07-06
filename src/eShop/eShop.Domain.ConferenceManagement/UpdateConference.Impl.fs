module eShop.Domain.ConferenceManagement.UpdateConference.Impl

open eShop.Infrastructure
open eShop.Domain.Shared
open eShop.Domain.ConferenceManagement.Common
open eShop.Domain.ConferenceManagement

// -----
// types
// -----

// step: validate
type ValidateConferenceInfo =
    UnvalidatedConferenceInfo -> Result<ValidatedConferenceInfo, ValidationError>

// step: update in db
type UpdateConferenceInfoInDb = ValidatedConferenceInfo -> Async<unit>

// step: create events
type CreateEvents = ValidatedConferenceInfo -> UpdateConferenceEvent list

// -----
// impl
// -----

let validateConferenceInfo: ValidateConferenceInfo =
    fun unvalidatedInfo ->
        result {
            let! id =
                unvalidatedInfo.Id
                |> ConferenceId.create "Id"
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
            let! startDate =
                unvalidatedInfo.StartDate
                |> Date.create "Start Date"
                |> Result.mapError ValidationError
            let! endDate =
                unvalidatedInfo.EndDate
                |> Date.create "End Date"
                |> Result.mapError ValidationError
            do! (startDate, endDate)
                ||> CreateConference.Impl.validateDateOrder
                |> Result.mapError ValidationError
            let validatedInfo: ValidatedConferenceInfo =
                { Id = id
                  Name = name
                  Description = description
                  Location = location
                  Tagline = tagline
                  TwitterSearch = twitterSearch
                  StartDate = startDate
                  EndDate = endDate }

            return validatedInfo
        }

let createConferenceUpdatedEvent (info: ValidatedConferenceInfo) : ConferenceUpdated = info

let createEvents: CreateEvents =
    fun validatedInfo ->
        let conferenceUpdated =
            validatedInfo
            |> createConferenceUpdatedEvent
            |> UpdateConferenceEvent.ConferenceUpdated

        [
            yield conferenceUpdated
        ]

let updateConference (updateConferenceInfoInDb: UpdateConferenceInfoInDb) : UpdateConference =
    fun cmd ->
        asyncResult {
            let! validatedInfo =
                validateConferenceInfo cmd.Data
                |> AsyncResult.ofResult
                |> AsyncResult.mapError UpdateConferenceError.Validation
            do! updateConferenceInfoInDb validatedInfo
                |> AsyncResult.ofAsync
            let events = validatedInfo |> createEvents

            return events
        }

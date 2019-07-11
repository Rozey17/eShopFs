module eShop.Domain.Conference.UnpublishConference.Impl

open eShop.Infrastructure
open eShop.Domain.Conference

// -----
// types
// -----

// step: apply unpublish
type ApplyUnpublish = Conference -> Conference

// step: create events
type CreateEvents = Conference -> UnpublishConferenceEvent list

// -----
// impl
// -----

// step: apply unpublish
let applyUnpublish: ApplyUnpublish =
    fun conference ->
        match conference with
        | PublishedConference (info, seats) ->
            UnpublishedConference(info=info, wasEverPublished=true, seats=seats)
        | unpublishedConference ->
            unpublishedConference

// step: create events
let createConferenceUnpublishedEvent conference : ConferenceUnpublished = conference

let createEvents: CreateEvents =
    fun conference ->
        let conferenceUnpublished =
            conference
            |> createConferenceUnpublishedEvent
            |> UnpublishConference.ConferenceUnpublished
        [
            yield conferenceUnpublished
        ]

// workflow
let unpublishConference
    (readSingleConference: ConferenceDb.ReadSingleConference)
    (markConferenceAsUnpublished: ConferenceDb.MarkConferenceAsUnpublished)
    : UnpublishConference =

        fun (ConferenceIdentifier (slug, accessCode)) ->
            asyncResult {
                let! identifier =
                    (slug, accessCode)
                    |> Validation.validateConferenceIdentifier
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError (ValidationError >> UnpublishConference.Validation)
                let! conference =
                    readSingleConference identifier
                    |> AsyncResult.mapError UnpublishConference.ConferenceNotFound

                let unpublishedConference = conference |> applyUnpublish

                do! markConferenceAsUnpublished unpublishedConference
                    |> AsyncResult.ofAsync

                let events = unpublishedConference |> createEvents
                return events
            }

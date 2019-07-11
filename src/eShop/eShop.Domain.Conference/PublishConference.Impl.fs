module eShop.Domain.Conference.PublishConference.Impl

open eShop.Infrastructure
open eShop.Domain.Conference

// -----
// types
// -----

// step: publish
type ApplyPublish = Conference -> Conference

// step: create events
type CreateEvents = Conference -> PublishConferenceEvent list

// -----
// impl
// -----

// step: apply publish
let applyPublish: ApplyPublish =
    fun conference ->
        match conference with
        | UnpublishedConference (info, _, seats) ->
            PublishedConference (info, seats)
        | publishedConference ->
            publishedConference

// step: create events
let createConferencePublishedEvent conference : ConferencePublished = conference

let createEvents: CreateEvents =
    fun conference ->
        let conferencePublished =
            conference
            |> createConferencePublishedEvent
            |> PublishConference.ConferencePublished
        [
            yield conferencePublished
        ]

// workflow
let publishConference
    (readSingleConference: ConferenceDb.ReadSingleConference)
    (markConferenceAsPublished: ConferenceDb.MarkConferenceAsPublished)
    : PublishConference =
        fun id ->
            asyncResult {
                let! id =
                    id
                    |> ConferenceId.create
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError (ValidationError >> PublishConference.Validation)

                let! conference =
                    readSingleConference id
                    |> AsyncResult.mapError PublishConference.ConferenceNotFound

                let publishedConference = conference |> applyPublish

                do! markConferenceAsPublished publishedConference
                    |> AsyncResult.ofAsync

                let events = publishedConference |> createEvents
                return events
            }

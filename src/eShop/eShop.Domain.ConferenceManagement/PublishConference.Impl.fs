module eShop.Domain.ConferenceManagement.PublishConference.Impl

open eShop.Domain.ConferenceManagement.Common

// -----
// types
// -----

// step: read single conference
type ReadSingleConference = UniqueSlug * AccessCode -> Async<Conference>

// step: publish
type ApplyPublishConference = Conference -> PublishedConference

// step: mark conference as published in db
type MarkConferenceAsPublishedInDb = PublishedConference -> Async<unit>

// step: create events
type CreateEvents = PublishedConference -> PublishConferenceEvent list

// -----
// impl
// -----

// step: publish
let applyPublishConference: ApplyPublishConference =
    fun conference ->
        match conference with
        | Unpublished (UnpublishedConference (info, _)) ->
            PublishedConference info
        | Published publishedConference ->
            publishedConference

// step: create events
let createConferencePublishedEvent conference : ConferencePublished = conference

let createEvents: CreateEvents =
    fun conference ->
        let conferencePublished =
            conference
            |> createConferencePublishedEvent
            |> PublishConferenceEvent.ConferencePublished
        [
            yield conferencePublished
        ]

// workflow
let publishConference
    (readSingleConference: ReadSingleConference)
    (markConferenceAsPublishedInDb: MarkConferenceAsPublishedInDb)
    : PublishConference =

    fun cmd ->
        async {
            let! conference = readSingleConference cmd.Data
            let publishedConference = conference |> applyPublishConference
            do! markConferenceAsPublishedInDb publishedConference

            let events = publishedConference |> createEvents
            return events
        }

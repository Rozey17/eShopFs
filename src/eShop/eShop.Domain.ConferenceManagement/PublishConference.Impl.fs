module eShop.Domain.ConferenceManagement.PublishConference.Impl

open eShop.Domain.ConferenceManagement.Common

// -----
// types
// -----

// step: read single conference
type ReadSingleConference = ConferenceId -> Async<Conference>

// step: publish
type ApplyPublishConference = Conference -> Conference

// step: mark conference as published in db
type MarkConferenceAsPublishedInDb = Conference -> Async<unit>

// step: create events
type CreateEvents = Conference -> PublishConferenceEvent list

// -----
// impl
// -----
let applyPublishConference conference =
    match conference with
    | Unpublished (UnpublishedConference (info, _)) ->
        Published (PublishedConference info)
    | published ->
        published

let createConferencePublishedEvent conference : ConferencePublished = conference |> Conference.id
let createEvents: CreateEvents =
    fun conference ->
        let conferencePublished =
            conference
            |> createConferencePublishedEvent
            |> PublishConferenceEvent.ConferencePublished
        [
            yield conferencePublished
        ]


let publishConference
    (readSingleConference: ReadSingleConference)
    (markConferenceAsPublishedInDb: MarkConferenceAsPublishedInDb)
    : PublishConference =

    fun cmd ->
        async {
            let! conference = readSingleConference cmd.Data
            let conference = conference |> applyPublishConference
            do! markConferenceAsPublishedInDb conference

            let events = conference |> createEvents
            return events
        }

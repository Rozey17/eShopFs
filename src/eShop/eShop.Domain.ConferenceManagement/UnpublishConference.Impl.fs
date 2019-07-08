module eShop.Domain.ConferenceManagement.UnpublishConference.Impl

open eShop.Domain.ConferenceManagement.Common

// -----
// types
// -----

// step: read single conference
type ReadSingleConference = UniqueSlug * AccessCode -> Async<Conference>

// step: unpublish
type ApplyUnpublishConference = Conference -> UnpublishedConference

// step: mark conference as unpublished in db
type MarkConferenceAsUnpublishedInDb = UnpublishedConference -> Async<unit>

// step: create events
type CreateEvents = UnpublishedConference -> UnpublishConferenceEvent list

// -----
// impl
// -----

// step: unpublish
let applyUnpublishConference: ApplyUnpublishConference =
    fun conference ->
        match conference with
        | Published (PublishedConference info) ->
            UnpublishedConference(info=info, wasEverPublished=true)
        | Unpublished unpublishedConference ->
            unpublishedConference

// step: create events
let createConferenceUnpublishedEvent unpublishedConference : ConferenceUnpublished
    = unpublishedConference

let createEvents: CreateEvents =
    fun conference ->
        let conferenceUnpublished =
            conference
            |> createConferenceUnpublishedEvent
            |> UnpublishConferenceEvent.ConferenceUnpublished
        [
            yield conferenceUnpublished
        ]

module eShop.Domain.Conference.DeleteSeat.Impl

open eShop.Domain.Conference
open eShop.Infrastructure

// -----
// types
// -----

type ApplyDeleteSeat = SeatTypeId -> Conference -> Result<Conference * SeatType, ConferenceWasEverPublishedError>

type CreateEvents = SeatType -> DeleteSeatEvent list

// -----
// impl
// -----

// apply
let applyDeleteSeat: ApplyDeleteSeat =
    fun seatTypeId conference ->
        let wasEverPublished = conference |> Conference.wasEverPublished
        if wasEverPublished then
            Error ConferenceWasEverPublishedError
        else
            let seats = conference |> Conference.seats
            let deletedSeat = seats |> List.find (fun it -> it.Id = seatTypeId)
            let remainingSeats = seats |> List.filter (fun it -> it.Id <> seatTypeId)
            let conference =
                match conference with
                | PublishedConference (info, _) ->
                    PublishedConference (info, remainingSeats)
                | UnpublishedConference (info, wasEverPublished, _) ->
                    UnpublishedConference (info, wasEverPublished, remainingSeats)
            Ok (conference, deletedSeat)

// create events
let createSeatDeletedEvent seat : SeatDeleted = seat
let createEvents: CreateEvents =
    fun seat ->
        let seatDeletedEvent =
            createSeatDeletedEvent seat
            |> DeleteSeatEvent.SeatDeleted
        [
            yield seatDeletedEvent
        ]


let deleteSeat
    (readSingleConferenceFromDb: ConferenceDb.ReadSingleConference)
    (deleteSeatInDb: ConferenceDb.DeleteSeat)
    : DeleteSeat =

        fun (conferenceId, seatTypeId) ->
            asyncResult {
                let! conferenceId =
                    conferenceId
                    |> ConferenceId.create
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError (ValidationError >> DeleteSeatError.Validation)
                let! seatTypeId =
                    seatTypeId
                    |> SeatTypeId.create
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError (ValidationError >> DeleteSeatError.Validation)

                let! conference =
                    readSingleConferenceFromDb conferenceId
                    |> AsyncResult.mapError DeleteSeatError.ConferenceNotFound

                let! _conference, deletedSeat =
                    conference
                    |> applyDeleteSeat seatTypeId
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError DeleteSeatError.ConferenceWasEverPublished

                do! deleteSeatInDb deletedSeat
                    |> AsyncResult.ofAsync

                let events = deletedSeat |> createEvents
                return events
            }

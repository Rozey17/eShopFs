namespace eShop.Domain.Conference.DeleteSeat

open System
open eShop.Infrastructure
open eShop.Domain.Conference

// input
type ConferenceId = Guid
type SeatTypeId = Guid
type DeleteSeatCommand = ConferenceId * SeatTypeId

// success output
type SeatDeleted = SeatType
type DeleteSeatEvent =
    | SeatDeleted of SeatDeleted

// error output
type ValidationError = ValidationError of string
type ConferenceWasEverPublishedError = ConferenceWasEverPublishedError
type DeleteSeatError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.RecordNotFound
    | ConferenceWasEverPublished of ConferenceWasEverPublishedError

// workflow
type DeleteSeat =
    DeleteSeatCommand -> AsyncResult<DeleteSeatEvent list, DeleteSeatError>

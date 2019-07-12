namespace eShop.Domain.Conference.UpdateSeat

open System
open eShop.Infrastructure
open eShop.Domain.Conference

// input
type UnvalidatedSeatType =
    { ConferenceId: Guid
      Id: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }
type UpdateSeatCommand = UnvalidatedSeatType

// success output
type SeatUpdated = SeatType
type PublishedSeatUpdated = SeatType
type UpdateSeatEvent =
    | SeatUpdated of SeatUpdated
    | PublishedSeatUpdated of PublishedSeatUpdated

// error output
type ValidationError = ValidationError of string
type UpdateSeatError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.RecordNotFound

// workflow
type UpdateSeat =
    UpdateSeatCommand -> AsyncResult<UpdateSeatEvent list, UpdateSeatError>


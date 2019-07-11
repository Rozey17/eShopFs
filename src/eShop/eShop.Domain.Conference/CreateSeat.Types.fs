namespace eShop.Domain.Conference.CreateSeat

open System
open eShop.Infrastructure
open eShop.Domain.Conference

// input
type UnvalidatedSeatType =
    { ConferenceId: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }
type CreateSeatCommand = UnvalidatedSeatType

// success output
type PublishedSeatCreated = SeatType
type SeatCreated = SeatType
type CreateSeatEvent =
    | PublishedSeatCreated of PublishedSeatCreated
    | SeatCreated of SeatCreated

// error output
type ValidationError = ValidationError of string
type CreateSeatError =
    | Validation of ValidationError
    | ConferenceNotFound of ConferenceDb.RecordNotFound

// workflow
type CreateSeat =
    UnvalidatedSeatType -> AsyncResult<CreateSeatEvent list, CreateSeatError>

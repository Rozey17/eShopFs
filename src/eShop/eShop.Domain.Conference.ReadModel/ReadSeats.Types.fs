namespace eShop.Domain.Conference.ReadModel.ReadSeats

open System

[<CLIMutable>]
type SeatTypeDTO =
    { ConferenceId: Guid
      Id: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }

type ConferenceIdentifier = string * string

type ReadSeats = ConferenceIdentifier -> Async<SeatTypeDTO seq>

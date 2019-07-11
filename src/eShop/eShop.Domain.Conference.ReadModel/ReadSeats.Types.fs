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

type ConferenceId = Guid

type ReadSeats = ConferenceId -> Async<SeatTypeDTO seq>

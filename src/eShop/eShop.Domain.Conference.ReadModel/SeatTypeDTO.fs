namespace eShop.Domain.Conference.ReadModel.SeatTypeDTO

open System

[<CLIMutable>]
type SeatTypeDTO =
    { ConferenceId: Guid
      Id: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }

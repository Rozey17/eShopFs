namespace eShop.Domain.Conference.Web.CreateSeat

open System
open eShop.Domain.Conference.CreateSeat

[<CLIMutable>]
type SeatFormDTO =
    { ConferenceId: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }

module SeatFormDTO =
    let toUnvalidatedSeatType (dto: SeatFormDTO) =
        let domainObj: UnvalidatedSeatType =
            { ConferenceId = dto.ConferenceId
              Name = dto.Name
              Description = dto.Description
              Quantity = dto.Quantity
              Price = dto.Price  }
        domainObj

namespace eShop.Domain.Conference.Web.CreateSeat

open System
open eShop.Domain.Conference.CreateSeat

[<CLIMutable>]
type SeatTypeFormDTO =
    { Name: string
      Description: string
      Quantity: int
      Price: decimal }

module SeatTypeFormDTO =
    let toUnvalidatedSeatType (conferenceId: Guid) (dto: SeatTypeFormDTO) =
        let domainObj: UnvalidatedSeatType =
            { ConferenceId = conferenceId
              Name = dto.Name
              Description = dto.Description
              Quantity = dto.Quantity
              Price = dto.Price  }
        domainObj

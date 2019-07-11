namespace eShop.Domain.Conference.Web.EditSeat

open System
open eShop.Domain.Conference.ReadModel.SeatTypeDTO

[<CLIMutable>]
type SeatTypeFormDTO =
    { ConferenceId: Guid
      Id: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }
module SeatTypeFormDTO =
    let fromSeatTypeDTO (dto: SeatTypeDTO) =
        let form: SeatTypeFormDTO =
            { ConferenceId = dto.ConferenceId
              Id = dto.Id
              Name = dto.Name
              Description = dto.Description
              Quantity = dto.Quantity
              Price = dto.Price  }
        form

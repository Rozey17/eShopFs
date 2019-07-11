namespace eShop.Domain.Conference.Web.CreateSeat

open System
open eShop.Domain.Common
open eShop.Domain.Conference
open eShop.Domain.Conference.CreateSeat
open eShop.Domain.Conference.ReadModel.SeatTypeDTO

// create form dto
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

// result dto
module SeatTypeDTO =
    let fromDomain (e: SeatCreated) =
        { ConferenceId = e.ConferenceId |> ConferenceId.value
          Id = e.Id |> SeatTypeId.value
          Name = e.Name |> Name.value
          Description = e.Description |> String250.value
          Quantity = e.Quantity |> UnitQuantity.value
          Price = e.Price |> Price.value }

// result dto
[<CLIMutable>]
type PublishedSeatCreatedDTO =
    { ConferenceId: Guid
      Id: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }
module PublishedSeatCreatedDTO =
    let fromDomain (e: PublishedSeatCreated) =
        { ConferenceId = e.ConferenceId |> ConferenceId.value
          Id = e.Id |> SeatTypeId.value
          Name = e.Name |> Name.value
          Description = e.Description |> String250.value
          Quantity = e.Quantity |> UnitQuantity.value
          Price = e.Price |> Price.value }

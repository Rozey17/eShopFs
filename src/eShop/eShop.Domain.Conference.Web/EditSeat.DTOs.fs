namespace eShop.Domain.Conference.Web.EditSeat

open System
open eShop.Domain.Common
open eShop.Domain.Conference
open eShop.Domain.Conference.ReadModel.SeatTypeDTO
open eShop.Domain.Conference.UpdateSeat

[<CLIMutable>]
type SeatTypeFormDTO =
    { ConferenceId: Guid
      Id: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }
module SeatTypeFormDTO =
    let fromReadModel (dto: SeatTypeDTO) =
        let form: SeatTypeFormDTO =
            { ConferenceId = dto.ConferenceId
              Id = dto.Id
              Name = dto.Name
              Description = dto.Description
              Quantity = dto.Quantity
              Price = dto.Price  }
        form

    let toUnvalidatedSeatType (form: SeatTypeFormDTO) =
        let domainObj: UnvalidatedSeatType =
            { ConferenceId = form.ConferenceId
              Id = form.Id
              Name = form.Name
              Description = form.Description
              Quantity = form.Quantity
              Price = form.Price }
        domainObj

// result dto
module SeatTypeDTO =
    let fromDomain (e: SeatUpdated) =
        let dto: SeatTypeDTO =
            { ConferenceId = e.ConferenceId |> ConferenceId.value
              Id = e.Id |> SeatTypeId.value
              Name = e.Name |> Name.value
              Description = e.Description |> String250.value
              Quantity = e.Quantity |> UnitQuantity.value
              Price = e.Price |> Price.value }
        dto

// result dto
[<CLIMutable>]
type PublishedSeatUpdatedDTO =
    { ConferenceId: Guid
      Id: Guid
      Name: string
      Description: string
      Quantity: int
      Price: decimal }
module PublishedSeatUpdatedDTO =
    let fromDomain (e: PublishedSeatUpdated) =
        { ConferenceId = e.ConferenceId |> ConferenceId.value
          Id = e.Id |> SeatTypeId.value
          Name = e.Name |> Name.value
          Description = e.Description |> String250.value
          Quantity = e.Quantity |> UnitQuantity.value
          Price = e.Price |> Price.value }
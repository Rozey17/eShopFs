namespace eShop.Domain.Conference.Web.PublishConference

open System
open eShop.Domain.Common
open eShop.Domain.Conference
open eShop.Domain.Conference.PublishConference

type ConferencePublishedDTO = { Id: Guid }
module ConferencePublishedDTO =
    let fromDomain (e: ConferencePublished) =
        { Id = e |> Conference.id |> ConferenceId.value }

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

type PublishConferenceEventDTO =
    | PublishedSeatCreatedDTO of PublishedSeatCreatedDTO
    | ConferencePublishedDTO of ConferencePublishedDTO
module PublishConferenceEventDTO =
    let fromDomain (e: PublishConferenceEvent) =
        match e with
        | ConferencePublished e ->
            ConferencePublishedDTO.fromDomain e
            |> PublishConferenceEventDTO.ConferencePublishedDTO
        | PublishedSeatCreated e ->
            PublishedSeatCreatedDTO.fromDomain e
            |> PublishConferenceEventDTO.PublishedSeatCreatedDTO

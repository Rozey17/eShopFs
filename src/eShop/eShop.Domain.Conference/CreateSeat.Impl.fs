module eShop.Domain.Conference.CreateSeat.Impl

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.Conference

// -----
// types
// -----

// step: validate
type ValidatedSeatType =
    { ConferenceId: ConferenceId
      Name: Name
      Description: String250
      Quantity: UnitQuantity
      Price: Price }

type ValidateSeatType = UnvalidatedSeatType -> Result<ValidatedSeatType, ValidationError>

// step: enrich
type EnrichValidatedSeatTypeWith = SeatTypeId -> ValidatedSeatType -> SeatType

// step: add seat to conference
type AddSeatToConference = SeatType -> Conference -> Conference

// step: create events
type CreateEvents = Conference * SeatType -> CreateSeatEvent list

// -----
// impl
// -----

// step: validate
let validateSeatType: ValidateSeatType =
    fun unvalidated ->
        result {
            let! conferenceId =
                unvalidated.ConferenceId
                |> ConferenceId.create
                |> Result.mapError ValidationError
            let! name =
                unvalidated.Name
                |> Name.create
                |> Result.mapError ValidationError
            let! description =
                unvalidated.Description
                |> String250.create "Description"
                |> Result.mapError ValidationError
            let! quantity =
                unvalidated.Quantity
                |> UnitQuantity.create "Quantity"
                |> Result.mapError ValidationError
            let! price =
                unvalidated.Price
                |> Price.create "Price"
                |> Result.mapError ValidationError
            let seatType: ValidatedSeatType =
                { ConferenceId = conferenceId
                  Name = name
                  Description = description
                  Quantity = quantity
                  Price = price }

            return seatType
        }

// step: enrich
let enrichWith: EnrichValidatedSeatTypeWith =
    fun id validated ->
        { ConferenceId = validated.ConferenceId
          Id = id
          Name = validated.Name
          Description = validated.Description
          Quantity = validated.Quantity
          Price = validated.Price }

// step: add seat to conference
let addSeat: AddSeatToConference =
    fun newSeatType conference ->
        let existingSeatTypes = conference |> Conference.seats
        let seats = newSeatType::existingSeatTypes
        match conference with
        | PublishedConference (info, _) ->
            PublishedConference (info, seats)
        | UnpublishedConference (info, wasEverPublished, _) ->
            UnpublishedConference (info, wasEverPublished, seats)

// step: create events
let createPublishedSeatCreatedEvent seat : PublishedSeatCreated = seat
let createSeatCreatedEvent seat : SeatCreated = seat

let createEvents: CreateEvents =
    fun (conference, newSeatType) ->
        let publishedSeatCreated =
            newSeatType
            |> createPublishedSeatCreatedEvent
            |> CreateSeatEvent.PublishedSeatCreated
        let seatCreated =
            newSeatType
            |> createSeatCreatedEvent
            |> CreateSeatEvent.SeatCreated
        let wasEverPublished = conference |> Conference.wasEverPublished
        [
            yield seatCreated

            // Don't publish new seats if the conference was never published
            // (and therefore is not published either).
            if wasEverPublished then yield publishedSeatCreated
        ]

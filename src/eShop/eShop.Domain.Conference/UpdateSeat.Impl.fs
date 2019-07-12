module eShop.Domain.Conference.UpdateSeat.Impl

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.Conference

// -----
// types
// -----

// step: validate
type ValidatedSeatType =
    { ConferenceId: ConferenceId
      Id: SeatTypeId
      Name: Name
      Description: String250
      Quantity: UnitQuantity
      Price: Price }

type ValidateSeatType = UnvalidatedSeatType -> Result<ValidatedSeatType, ValidationError>

// apply update seat
type ApplyUpdateSeat = ValidatedSeatType -> Conference -> Conference

// step: create events
type CreateEvents = Conference * SeatType -> UpdateSeatEvent list


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
            let! id =
                unvalidated.Id
                |> SeatTypeId.create
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
                  Id = id
                  Name = name
                  Description = description
                  Quantity = quantity
                  Price = price }

            return seatType
        }

// step: apply update
let applyUpdate: ApplyUpdateSeat =
    fun validated conference ->
        let oldSeats = conference |> Conference.seats
        let newSeats =
            oldSeats
            |> List.map (fun it ->
                if it.Id = validated.Id then
                    { it with
                        Name = validated.Name
                        Description = validated.Description
                        Price = validated.Price
                        Quantity = validated.Quantity }
                else
                    it
                )
        match conference with
        | PublishedConference (info, _) ->
            PublishedConference (info, newSeats)
        | UnpublishedConference (info, wasEverPublished, _) ->
            UnpublishedConference (info, wasEverPublished, newSeats)

// step: create events

let createPublishedSeatUpdatedEvent seat : PublishedSeatUpdated = seat
let createSeatUpdatedEvent seat : SeatUpdated = seat

let createEvents: CreateEvents =
    fun (conference, seat) ->
        let seatUpdated =
            seat
            |> createSeatUpdatedEvent
            |> UpdateSeatEvent.SeatUpdated
        let publishedSeatUpdated =
            seat
            |> createPublishedSeatUpdatedEvent
            |> UpdateSeatEvent.PublishedSeatUpdated
        let wasEverPublished = conference |> Conference.wasEverPublished
        [
            yield seatUpdated

            // Don't publish seat updates if the conference was never published
            // (and therefore is not published either).
            if wasEverPublished then yield publishedSeatUpdated
        ]

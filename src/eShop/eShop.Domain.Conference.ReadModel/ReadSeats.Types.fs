namespace eShop.Domain.Conference.ReadModel.ReadSeats

open System
open eShop.Domain.Conference.ReadModel.SeatTypeDTO

type ConferenceId = Guid

type ReadSeats = ConferenceId -> Async<SeatTypeDTO seq>

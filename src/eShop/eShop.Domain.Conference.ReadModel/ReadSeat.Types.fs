namespace eShop.Domain.Conference.ReadModel.ReadSeat

open System
open eShop.Infrastructure
open eShop.Domain.Conference.ReadModel.SeatTypeDTO

type ConferenceId = Guid
type SeatTypeId = Guid

type RecordNotFound = RecordNotFound

type ReadSeat = ConferenceId * SeatTypeId -> AsyncResult<SeatTypeDTO, RecordNotFound>

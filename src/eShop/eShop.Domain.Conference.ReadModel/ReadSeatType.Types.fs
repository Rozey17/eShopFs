namespace eShop.Domain.Conference.ReadModel.ReadSeatType

open System
open eShop.Infrastructure
open eShop.Domain.Conference.ReadModel.SeatTypeDTO

type ConferenceId = Guid
type SeatTypeId = Guid

type RecordNotFound = RecordNotFound

type ReadSeatType = ConferenceId * SeatTypeId -> AsyncResult<SeatTypeDTO, RecordNotFound>

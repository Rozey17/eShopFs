namespace eShop.Domain.Conference.ConferenceDb

open eShop.Infrastructure
open eShop.Domain.Conference

type RecordNotFound = RecordNotFound

type ReadSingleConference = UniqueSlug * AccessCode -> AsyncResult<Conference, RecordNotFound>
type CheckSlugExists = UniqueSlug -> Async<bool>
type InsertConference = Conference -> Async<unit>
type UpdateConference = Conference -> Async<unit>
type MarkConferenceAsPublished = Conference -> Async<unit>
type MarkConferenceAsUnpublished = Conference -> Async<unit>

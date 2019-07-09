namespace eShop.Domain.Conference

open eShop.Infrastructure

[<RequireQualifiedAccess>]
module ConferenceDb =

    type NotFound = NotFound

    type ReadSingleConference = UniqueSlug * AccessCode -> AsyncResult<Conference, NotFound>
    type CheckSlugExists = UniqueSlug -> Async<bool>
    type InsertConference = Conference -> Async<unit>
    type UpdateConference = Conference -> Async<unit>
    type MarkConferenceAsPublished = Conference -> Async<unit>
    type MarkConferenceAsUnpublished = Conference -> Async<unit>

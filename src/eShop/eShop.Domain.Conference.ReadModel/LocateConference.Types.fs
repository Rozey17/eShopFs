namespace eShop.Domain.Conference.ReadModel.LocateConference

open eShop.Infrastructure

// input
type ConferenceIdentifier =
    { Email: string
      AccessCode: string }

// output
[<CLIMutable>]
type ConferenceDTO =
    { Email: string
      AccessCode: string
      Slug: string }

// error
type RecordNotFound = RecordNotFound

// query
type LocateConference = ConferenceIdentifier -> AsyncResult<ConferenceDTO, RecordNotFound>

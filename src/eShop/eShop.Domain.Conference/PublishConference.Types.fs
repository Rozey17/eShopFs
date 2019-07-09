namespace eShop.Domain.Conference.PublishConference

open eShop.Domain.Conference

// input
type ConferenceIdentifier = ConferenceIdentifier of slug:string * accessCode:string
type PublishConferenceCommand = ConferenceIdentifier

// success output
type ConferencePublished = Conference
type PublishConferenceEvent =
    | ConferencePublished of ConferencePublished

// workflow
type PublishConference =
    PublishConferenceCommand -> Async<PublishConferenceEvent list>


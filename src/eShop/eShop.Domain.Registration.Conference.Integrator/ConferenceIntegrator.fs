module eShop.Domain.Registration.Conference.Integrator.ConferenceIntegrator

open eShop.Infrastructure.Bus
open eShop.Domain.Conference.Web.CreateConference
open eShop.Domain.Conference.Web.PublishConference
open eShop.Domain.Conference.Web.UnpublishConference

let initialise () =
    let subId = SubscriptionId "Registration"
    Bus.Subscribe<ConferenceCreatedDTO> subId CreateConferenceEventListener.execute
    Bus.Subscribe<PublishConferenceEventDTO> subId PublishConferenceEventListener.execute
    Bus.Subscribe<ConferenceUnpublishedDTO> subId UnpublishConferenceEventListener.execute

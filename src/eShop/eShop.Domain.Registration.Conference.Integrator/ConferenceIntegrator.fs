module eShop.Domain.Registration.Conference.Integrator.ConferenceIntegrator

open eShop.Infrastructure.Bus
open eShop.Domain.Conference.Web.CreateConference
open eShop.Domain.Conference.Web.PublishConference

let initialise () =
    let subId = SubscriptionId "Registration"
    Bus.Subscribe<ConferenceCreatedDTO> subId CreateConferenceListener.execute
    Bus.Subscribe<PublishConferenceEventDTO> subId PublishConferenceListener.execute

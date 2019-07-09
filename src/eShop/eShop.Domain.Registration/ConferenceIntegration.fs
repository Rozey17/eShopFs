module eShop.Domain.Registration.ConferenceIntegration

open eShop.Infrastructure.Bus
open eShop.Domain.ConferenceManagement.CreateConference

let OnConferenceCreated (e: ConferenceCreated) =
    async {
        printf "hello: %A" e
    }

let initialize() =
    let subId = SubscriptionId "Registration"
    Bus.Subscribe<ConferenceCreated> subId OnConferenceCreated

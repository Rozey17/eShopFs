module eShop.Domain.Registration.Conference.Integrator

open Npgsql
open eShop.Infrastructure
open eShop.Infrastructure.Bus
open eShop.Domain.Conference.Web.CreateConference

let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"

let OnConferenceCreated (event: ConferenceCreatedDTO) =
    async {
        use connection = new NpgsqlConnection(connStr)
        let sql =
            """
            insert into
                r.conference
                (
                    id,
                    name,
                    description,
                    location,
                    tagline,
                    slug,
                    twitter_search,
                    start_date
                )
                values
                (
                    @Id,
                    @Name,
                    @Description,
                    @Location,
                    @Tagline,
                    @Slug,
                    @TwitterSearch,
                    @StartDate
                )
            """
        do! Db.parameterizedExecuteAsync connection sql event
    }

let initialise () =
    let subId = SubscriptionId "Registration"
    Bus.Subscribe<ConferenceCreatedDTO> subId OnConferenceCreated

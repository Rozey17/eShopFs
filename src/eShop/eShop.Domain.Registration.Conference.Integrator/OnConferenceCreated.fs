module eShop.Domain.Registration.Conference.Integrator.OnConferenceCreated

open Npgsql
open eShop.Infrastructure
open eShop.Domain.Conference.Web.CreateConference

let execute (e: ConferenceCreatedDTO) =
    async {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
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
                    start_date,
                    is_published
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
                    @StartDate,
                    @IsPublished
                )
            """
        do! Db.parameterizedExecuteAsync connection sql e
    }

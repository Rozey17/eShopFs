module eShop.Domain.Registration.Conference.Integrator.PublishConferenceListener

open Npgsql
open eShop.Infrastructure
open eShop.Domain.Conference.Web.PublishConference

let execute (e: PublishConferenceEventDTO) =
    async {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        match e with
        | ConferencePublishedDTO e ->
            let sql =
                """
                update r.conference
                   set is_published = 't'
                 where id = @Id
                """
            do! Db.parameterizedExecuteAsync connection sql e
    }

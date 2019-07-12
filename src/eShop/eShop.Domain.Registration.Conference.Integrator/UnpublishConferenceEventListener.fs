module eShop.Domain.Registration.Conference.Integrator.UnpublishConferenceEventListener

open Npgsql
open eShop.Infrastructure
open eShop.Domain.Conference.Web.UnpublishConference

let execute (e: ConferenceUnpublishedDTO) =
    async {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let sql =
            """
            update r.conference
               set is_published = 'f'
             where id = @Id
            """
        do! Db.parameterizedExecuteAsync connection sql e
    }

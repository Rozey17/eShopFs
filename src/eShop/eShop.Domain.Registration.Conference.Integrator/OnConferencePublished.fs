module eShop.Domain.Registration.Conference.Integrator.OnConferencePublished

open Npgsql
open eShop.Infrastructure
open eShop.Domain.Conference.Web.PublishConference

let execute (e: ConferencePublishedDTO) =
    async {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)
        let sql =
            """
            update r.conference
               set is_published = 't'
             where id = @Id
            """
        do! Db.parameterizedExecuteAsync connection sql e
    }

module eShop.Domain.ConferenceManagement.PublishConference.Db

open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

module MarkConferenceAsPublishedInDb =

    let execute connection conference =
        let id = conference |> Conference.id |> ConferenceId.value
        let sql =
            """
            update conference
               set published = 't',
                   was_ever_published = 't'
             where id = @Id
            """
        let param = {| Id = id |}
        Db.parameterizedExecuteAsync connection sql param

module eShop.Domain.ConferenceManagement.PublishConference.Db

open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

module MarkConferenceAsPublishedInDb =

    let execute connection (PublishedConference info) =
        let id = info.Id |> ConferenceId.value
        let sql =
            """
            update conference
               set is_published = 't',
                   was_ever_published = 't'
             where id = @Id
            """
        let param = {| Id = id |}
        Db.parameterizedExecuteAsync connection sql param

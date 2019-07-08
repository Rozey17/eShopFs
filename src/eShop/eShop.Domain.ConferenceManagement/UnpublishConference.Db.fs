module eShop.Domain.ConferenceManagement.UnpublishConference.Db

open eShop.Infrastructure
open eShop.Domain.ConferenceManagement.Common

module MarkConferenceAsUnpublishedInDb =

    let execute connection (UnpublishedConference (info, _)) =
        let id = info.Id |> ConferenceId.value
        let sql =
            """
            update conference
               set is_published = 'f'
             where id = @Id
            """
        let param = {| Id = id |}
        Db.parameterizedExecuteAsync connection sql param

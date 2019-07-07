module eShop.Domain.ConferenceManagement.LocateConference.Db

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

module ReadConferenceByEmailAndAccessCode =

    [<CLIMutable>]
    type QueryResult =
        { slug: string
          access_code: string
          owner_email: string }

    let query connection email accessCode =
        let email = email |> EmailAddress.value
        let accessCode = accessCode |> AccessCode.value

        let sql =
            """
            select owner_email,
                   slug,
                   access_code
              from conference
             where owner_email = @Email
               and access_code = @AccessCode
            """
        let param = {| Email = email; AccessCode = accessCode |}

        async {
            let! result = Db.tryParameterizedQuerySingleAsync<QueryResult> connection sql param
            match result with
            | Some record ->
                let dto =
                    {| Email = record.owner_email
                       Slug = record.slug
                       AccessCode = record.access_code |}
                return Some dto

            | None ->
                return None
        }

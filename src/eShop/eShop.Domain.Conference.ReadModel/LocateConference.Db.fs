module eShop.Domain.Conference.ReadModel.LocateConference.Db

open eShop.Infrastructure

[<CLIMutable>]
type ConferenceDTO =
    { slug: string
      access_code: string
      owner_email: string }

let locateConference connection : LocateConference =
    fun { Email=email; AccessCode=accessCode } ->
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
            let! queryResult = Db.tryParameterizedQuerySingleAsync<ConferenceDTO> connection sql param
            match queryResult with
            | Some record ->
                let dto =
                    { Email = record.owner_email
                      AccessCode = record.access_code
                      Slug = record.slug }
                return Ok dto
            | None ->
                return Error RecordNotFound
        }

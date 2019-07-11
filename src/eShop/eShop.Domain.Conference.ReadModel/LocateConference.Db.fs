module eShop.Domain.Conference.ReadModel.LocateConference.Db

open eShop.Infrastructure

let locateConference connection : LocateConference =
    fun { Email=email; AccessCode=accessCode } ->
        let sql =
            """
            select owner_email as OwnerEmail,
                   slug as Slug,
                   access_code as AccessCode
              from cm.conference
             where owner_email = @Email
               and access_code = @AccessCode
            """
        let param = {| Email = email; AccessCode = accessCode |}

        async {
            let! queryResult = Db.tryParameterizedQuerySingleAsync<ConferenceDTO> connection sql param
            match queryResult with
            | Some record ->
                return Ok record
            | None ->
                return Error RecordNotFound
        }

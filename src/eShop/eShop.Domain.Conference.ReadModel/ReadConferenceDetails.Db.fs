module eShop.Domain.Conference.ReadModel.ReadConferenceDetails.Db

open eShop.Infrastructure

let readConferenceDetails connection : ReadConferenceDetails =
    fun (slug, accessCode) ->
        let sql =
            """
            select id as Id,
                   name as Name,
                   description as Description,
                   location as Location,
                   tagline as Tagline,
                   slug as Slug,
                   twitter_search as TwitterSearch,
                   start_date as StartDate,
                   end_date as EndDate,
                   access_code as AccessCode,
                   owner_name as OwnerName,
                   owner_email as OwnerEmail,
                   was_ever_published as WasEverPublished,
                   is_published as IsPublished
              from cm.conference
             where slug = @Slug
               and access_code = @AccessCode
            """
        let param = {| Slug = slug; AccessCode = accessCode |}

        async {
            let! readResult = Db.tryParameterizedQuerySingleAsync<ConferenceDetailsDTO> connection sql param
            match readResult with
            | Some record ->
                return Ok record
            | None ->
                return Error RecordNotFound
        }

module eShop.Domain.Registration.Conference.ReadModel.ReadConferenceDetails.Db

open eShop.Infrastructure

let readConferenceDetails connection : ReadConferenceDetails =
    fun slug ->
        let sql =
            """
            select id as Id,
                   slug as Slug,
                   name as Name,
                   description as Description,
                   location as Location,
                   tagline as Tagline,
                   twitter_search as TwitterSearch,
                   start_date as StartDate
              from r.conference
             where slug = @Slug
            """
        let param = {| Slug = slug |}

        async {
            let! queryResult = Db.tryParameterizedQuerySingleAsync<ConferenceDetailsDTO> connection sql param
            match queryResult with
            | Some record ->
                return Ok record
            | None ->
                return Error RecordNotFound
        }

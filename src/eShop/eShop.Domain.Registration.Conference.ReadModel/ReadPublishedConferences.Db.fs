module eShop.Domain.Registration.Conference.ReadModel.ReadPublishedConferences.Db

open eShop.Infrastructure

let readPublishedConferences connection : ReadPublishedConferences =
    fun () ->
        let sql =
            """
            select id,
                   slug,
                   name,
                   tagline
              from r.conference
             where is_published = 't'
            """
        Db.queryAsync<ConferenceEntryDTO> connection sql

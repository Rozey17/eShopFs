module eShop.Domain.Registration.Conference.ReadModel.ReadPublishedConferences.Db

open System
open eShop.Infrastructure

type ConferenceEntryDbDTO =
    { id: Guid
      slug: string
      name: string
      tagline: string }

module ConferenceEntryDbDTO =

    let toConferenceEntryDTO record =
        { Id = record.id
          Slug = record.slug
          Name = record.name
          Tagline = record.tagline }

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
        async {
            let! result = Db.queryAsync<ConferenceEntryDbDTO> connection sql
            let dtos = result |> Seq.map ConferenceEntryDbDTO.toConferenceEntryDTO
            return dtos
        }

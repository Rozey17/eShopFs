module eShop.Domain.Conference.ReadModel.ReadSeats.Db

open eShop.Infrastructure

let readSeats connection : ReadSeats =
    fun (slug, accessCode) ->
        let sql =
            """
            select conference_id as ConferenceId,
                   id as Id,
                   name as Name,
                   description as Description,
                   quantity as Quantity,
                   price as Price
              from cm.seat
             where slug = @Slug
               and access_code = @AccessCode
            """
        let param = {| Slug = slug; AccessCode = accessCode |}
        Db.parameterizedQueryAsync<SeatTypeDTO> connection sql param

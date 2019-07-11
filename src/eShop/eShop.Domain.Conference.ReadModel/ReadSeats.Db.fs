module eShop.Domain.Conference.ReadModel.ReadSeats.Db

open eShop.Infrastructure

let readSeats connection : ReadSeats =
    fun id ->
        let sql =
            """
            select conference_id as ConferenceId,
                   id as Id,
                   name as Name,
                   description as Description,
                   quantity as Quantity,
                   price as Price
              from cm.seat
             where id = @Id
            """
        let param = {| Id = id |}
        Db.parameterizedQueryAsync<SeatTypeDTO> connection sql param

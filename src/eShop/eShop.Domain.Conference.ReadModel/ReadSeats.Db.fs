module eShop.Domain.Conference.ReadModel.ReadSeats.Db

open eShop.Infrastructure
open eShop.Domain.Conference.ReadModel.SeatTypeDTO

let readSeats connection : ReadSeats =
    fun conferenceId ->
        let sql =
            """
            select conference_id as ConferenceId,
                   id as Id,
                   name as Name,
                   description as Description,
                   quantity as Quantity,
                   price as Price
              from cm.seat
             where conference_id = @ConferenceId
            """
        let param = {| ConferenceId = conferenceId |}
        Db.parameterizedQueryAsync<SeatTypeDTO> connection sql param

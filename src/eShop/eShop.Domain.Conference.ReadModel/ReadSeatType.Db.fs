module eShop.Domain.Conference.ReadModel.ReadSeatType.Db

open eShop.Infrastructure
open eShop.Domain.Conference.ReadModel.SeatTypeDTO

let readSeatType connection : ReadSeatType =
    fun (conferenceId, id) ->
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
               and id = @Id
            """
        let param = {| ConferenceId = conferenceId; Id = id |}

        async {
            let! queryResult = Db.tryParameterizedQuerySingleAsync<SeatTypeDTO> connection sql param
            match queryResult with
            | Some record ->
                return Ok record
            | _ ->
                return Error RecordNotFound
        }

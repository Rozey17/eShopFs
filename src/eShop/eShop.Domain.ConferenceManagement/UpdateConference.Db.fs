module eShop.Domain.ConferenceManagement.UpdateConference.Db

open eShop.Infrastructure
open eShop.Domain.Common
open eShop.Domain.ConferenceManagement.Common

module UpdateConferenceInDb =

    module ConferenceDbDTO =

        let fromDomain conference =
            let info = conference |> Conference.info

            {| Id = info.Id |> ConferenceId.value
               Name = info.Name |> String250.value
               Description = info.Description |> NotEmptyString.value
               Location = info.Location |> String250.value
               Tagline = info.Tagline |> Option.map String250.value |> Option.defaultValue null
               TwitterSearch = info.TwitterSearch |> Option.map String250.value |> Option.defaultValue null
               StartDate = info.StartAndEnd |> StartAndEnd.startDateValue
               EndDate = info.StartAndEnd |> StartAndEnd.endDateValue |}

    let execute connection conference =
        let sql =
            """
            update conference
               set name = @Name,
                   description = @Description,
                   location = @Location,
                   tagline = @Tagline,
                   twitter_search = @TwitterSearch,
                   start_date = @StartDate,
                   end_date = @EndDate
             where id = @Id
            """
        let dto = ConferenceDbDTO.fromDomain conference
        Db.parameterizedExecuteAsync connection sql dto

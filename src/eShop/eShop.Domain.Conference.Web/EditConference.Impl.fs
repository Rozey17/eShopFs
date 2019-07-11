module eShop.Domain.Conference.Web.EditConference.Impl

open System
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc.ModelBinding
open Giraffe
open Giraffe.Razor
open Npgsql
open eShop.Domain.Conference
open eShop.Domain.Conference.Web
open eShop.Domain.Conference.UpdateConference
open eShop.Domain.Conference.ReadModel.ReadConferenceDetails

// dto
[<CLIMutable>]
type EditConferenceFormDTO =
    { Id: Guid
      Name: string
      Description: string
      Location: string
      Tagline: string
      Slug: string
      TwitterSearch: string
      StartDate: DateTime
      EndDate: DateTime
      AccessCode: string
      OwnerName: string
      OwnerEmail: string
      WasEverPublished: bool
      IsPublished: bool }

module EditConferenceFormDTO =

    let fromConferenceDetailsDTO (details: ConferenceDetailsDTO) =
        let form: EditConferenceFormDTO =
            { Id = details.Id
              Name = details.Name
              Description = details.Description
              Location = details.Location
              Tagline = details.Tagline
              Slug = details.Slug
              TwitterSearch = details.TwitterSearch
              StartDate = details.StartDate
              EndDate = details.EndDate
              AccessCode = details.AccessCode
              OwnerName = details.OwnerName
              OwnerEmail = details.OwnerEmail
              WasEverPublished = details.WasEverPublished
              IsPublished = details.IsPublished }
        form

    /// Used when importing an Conference Form from outside world into the domain
    let toUnvalidatedConferenceInfo (dto: EditConferenceFormDTO) =
        let domainObj: UnvalidatedConferenceInfo =
            { Id = dto.Id
              Name = dto.Name
              Tagline = dto.Tagline
              Location = dto.Location
              TwitterSearch = dto.TwitterSearch
              Description = dto.Description
              StartDate = dto.StartDate
              EndDate = dto.EndDate }
        domainObj

// get
let renderEditConferenceView next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx

        let! result = Db.readConferenceDetails connection (slug, accessCode)

        match result with
        | Ok details ->
            let viewData = dict [("Slug", box details.Slug); ("AccessCode", box details.AccessCode)]
            let form = EditConferenceFormDTO.fromConferenceDetailsDTO details
            return! razorHtmlView "EditConference" (Some form) (Some viewData) None next ctx
        | _ ->
            return! text "bad request" next ctx
    }

// post
let updateConference next (ctx: HttpContext) =
    task {
        let connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=eshop"
        use connection = new NpgsqlConnection(connStr)

        let slug, accessCode = Common.exnQueryStringValue ctx

        let! form = ctx.BindFormAsync<EditConferenceFormDTO>()
        let unvalidatedInfo = form |> EditConferenceFormDTO.toUnvalidatedConferenceInfo
        let identifier = ConferenceIdentifier (form.Slug, form.AccessCode)
        let cmd = identifier, unvalidatedInfo

        let readSingleConference = ConferenceDb.Impl.ReadSingleConference.query connection
        let updateConference = ConferenceDb.Impl.UpdateConference.execute connection
        let workflow = Impl.updateConference readSingleConference updateConference

        let! result = workflow cmd

        match result with
        | Ok [ (ConferenceUpdated _) ] ->
            let url = sprintf "/conferences/details?slug=%s&access_code=%s" slug accessCode
            return! redirectTo false url next ctx

        | Error (Validation (ValidationError error)) ->
            let modelState = ModelStateDictionary()
            modelState.AddModelError("", error)
            let viewData = dict [("Slug", box slug); ("AccessCode", box accessCode)]
            return! razorHtmlView "EditConference" (Some form) (Some viewData) (Some modelState) next ctx

        | _ ->
            return! text "Unknown Error" next ctx
    }
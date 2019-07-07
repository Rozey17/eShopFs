namespace eShop.Domain.ConferenceManagement.Common

open System
open System.Text.RegularExpressions
open eShop.Infrastructure
open eShop.Domain.Shared

/// Constrained range from DateTime.Now to DateTime.Now + 1 year
type Date = private Date of DateTime
module Date =
    let value (Date v) = v

    let create fieldName dt =
        if dt < DateTime.Now then
            Error (sprintf "%s: must not happen in the past" fieldName)
        else if dt > DateTime.Now.AddYears(1) then
            Error (sprintf "%s: is too far in the future" fieldName)
        else
            Ok (Date dt)

type StartAndEnd = private StartAndEnd of startDate:Date * endDate:Date
module StartAndEnd =
    let startDateValue (StartAndEnd (startDate, _)) = startDate |> Date.value
    let endDateValue (StartAndEnd (_, endDate)) = endDate |> Date.value

    let create (startDate, endDate) =
        result {
            let! startDate = startDate |> Date.create "Start Date"
            let! endDate = endDate |> Date.create "End Date"
            do! if startDate > endDate then Error "StartDate can not come after EndDate"
                else Ok ()

            return StartAndEnd (startDate, endDate)
        }

/// Unique id of a conference
type ConferenceId = private ConferenceId of Guid
module ConferenceId =
    let value (ConferenceId v) = v

    let generate () =
        ConferenceId (Guid.NewGuid())

    let create fieldName guid =
        if guid = Guid.Empty then
            Error (sprintf "%s: must not be empty" fieldName)
        else
            Ok (ConferenceId guid)

/// Slug has a specific format, no more than 250 chars, not null
type UniqueSlug = private UniqueSlug of string
module UniqueSlug =
    let value (UniqueSlug v) = v

    let create str =
        if String.IsNullOrEmpty str then
            Error "Slug: must not be null or empty"
        else if str.Length > 250 then
            Error "Slug: must not be more than 250 chars"
        else if not (Regex.IsMatch(str, @"^\w+$")) then
            Error "Slug: has invalid format"
        else
            Ok (UniqueSlug str)

/// Access code is a generated string of 6 chars
type AccessCode = private AccessCode of string
module AccessCode =
    let value (AccessCode v) = v

    let generate () =
        AccessCode "abcdef" // TODO: replace real logic

    let create (str: string) =
        if str.Length <> 6 then
            Error "Access Code: has invalid format"
        else
            Ok (AccessCode str)

/// Once created, it can not be edited
type NotEditable<'a> = NotEditable of 'a

/// It is generated
type Generated<'a> = Generated of 'a

module NotEditableUniqueSlug =
    let value (NotEditable (UniqueSlug v)) = v

module GeneratedAndNotEditableAccessCode =
    let value (NotEditable (Generated (AccessCode v))) = v

/// Info of conference owner
type OwnerInfo =
    { Name: String250
      Email: EmailAddress }

module NotEditableOwnerInfo =
    let name (NotEditable { Name = name }) = name |> String250.value
    let email (NotEditable { Email = email }) = email |> EmailAddress.value

/// Conference info
type ConferenceInfo =
    { Id: ConferenceId
      Name: String250
      Description: NotEmptyString
      Location: String250
      Tagline: String250 option
      Slug: NotEditable<UniqueSlug>
      TwitterSearch: String250 option
      StartAndEnd: StartAndEnd
      AccessCode: NotEditable<Generated<AccessCode>>
      Owner: NotEditable<OwnerInfo> }

type WasNeverPublished = bool

type Conference =
    | UnpublishedConference of info:ConferenceInfo * canDeleteSeat:WasNeverPublished
    | PublisedConference of info:ConferenceInfo

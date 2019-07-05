namespace eShop.Domain.ConferenceManagement.Common

open System
open System.Text.RegularExpressions
open eShop.Domain.Shared

/// Constrained range from DateTime.Now to DateTime.Now + 1 year
type Date = private Date of DateTime

/// Unique id of a conference
type ConferenceId = private ConferenceId of Guid

/// Slug has a specific format, no more than 250 chars, not null
type UniqueSlug = private UniqueSlug of string

/// Access code is a generated string of 6 chars
type AccessCode = private AccessCode of string

/// Once created, it can not be edited
type NotEditable<'a> = NotEditable of 'a

/// It is generated and can not be edited
type GeneratedAndNotEditable<'a> = GeneratedAndNotEditable of 'a

/// Info of conference owner
type OwnerInfo =
    { Name: String250
      Email: EmailAddress }

/// Conference info
type ConferenceInfo =
    { Id: ConferenceId
      Name: String250
      Description: NotEmptyString
      Location: String250
      Tagline: String250 option
      Slug: NotEditable<UniqueSlug>
      TwitterSearch: String250 option
      StartDate: Date
      EndDate: Date
      AccessCode: GeneratedAndNotEditable<AccessCode>
      Owner: OwnerInfo }

type WasNeverPublished = bool

type Conference =
    | UnpublishedConference of info:ConferenceInfo * canDeleteSeat:WasNeverPublished
    | PublisedConference of info:ConferenceInfo

module Date =
    let value (Date v) = v

    let create fieldName dt =
        if dt < DateTime.Now then
            Error (sprintf "%s must not happen in the past" fieldName)
        else if dt > DateTime.Now.AddYears(1) then
            Error (sprintf "%s is too far in the future" fieldName)
        else
            Ok (Date dt)

module ConferenceId =
    let value (ConferenceId v) = v

    let generate () =
        ConferenceId (Guid.NewGuid())

module UniqueSlug =
    let value (UniqueSlug v) = v

    let create fieldName str =
        if String.IsNullOrEmpty str then
            Error (sprintf "%s must not be null or empty" fieldName)
        else if str.Length > 250 then
            Error (sprintf "%s must not be more than 250 chars" fieldName)
        else if not (Regex.IsMatch(str, @"^\w+$")) then
            Error (sprintf "%s has invalid format" fieldName)
        else
            Ok (UniqueSlug str)

module AccessCode =
    let value (AccessCode v) = v

    let generate () =
        AccessCode "abcdef" // TODO: replace real logic

module NotEditableUniqueSlug =
    let value (NotEditable (UniqueSlug v)) = v

    let create fieldName str =
        UniqueSlug.create fieldName str
        |> Result.map NotEditable

module GeneratedAndNotEditableAccessCode =
    let value (GeneratedAndNotEditable (AccessCode v)) = v

    let generate () =
        AccessCode.generate() |> GeneratedAndNotEditable

    let create fieldName (str: string) =
        if str.Length <> 6 then
            Error (sprintf "%s has invalid format" fieldName)
        else
            Ok (GeneratedAndNotEditable (AccessCode str))

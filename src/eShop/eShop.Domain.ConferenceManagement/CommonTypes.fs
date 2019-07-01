module eShop.Domain.ConferenceManagement.CommonTypes

open System
open System.Text.RegularExpressions
open eShop.Domain.Shared.Types

/// Constrained range from DateTime.Now to DateTime.Now + 1 year
type Date = private Date of DateTime

/// Unique id of a conference
type ConferenceId = private ConferenceId of Guid

/// Slug has a specific format, no more than 250 chars, not null
type Slug = private Slug of string

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
      Slug: NotEditable<Slug>
      TwitterSearch: String250 option
      StartDate: Date
      EndDate: Date
      AccessCode: GeneratedAndNotEditable<AccessCode>
      Owner: OwnerInfo }

type WasEverPublished = bool

type Conference = Conference of Info:ConferenceInfo * CanNotDeleteSeat:WasEverPublished

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

module ConferenceSlug =
    let value (Slug v) = v
    let create str =
        if String.IsNullOrEmpty str then
            Error "slug must not be null or empty"
        else if str.Length > 250 then
            Error "slug must not be more than 250 chars"
        else if not (Regex.IsMatch(str, @"^\w+$")) then
            Error "slug has invalid format"
        else
            Ok (Slug str)

module AccessCode =
    let value (AccessCode v) = v
    let generate () =
        "abcdef" // TODO: replace real logic

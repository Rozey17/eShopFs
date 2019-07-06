namespace eShop.Domain.ConferenceManagement.ConferenceDetails

open System

type ConferenceDetailsDTO =
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
      IsPublished: bool }

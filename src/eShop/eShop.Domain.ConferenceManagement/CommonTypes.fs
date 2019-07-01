module eShop.Domain.ConferenceManagement.CommonTypes

open eShop.Domain.Shared.Types

type ConferenceInfo =
    { Name: String250
      Description: NotEmptyString }
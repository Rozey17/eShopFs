module eShop.Domain.Conference.Validation

open eShop.Infrastructure

let validateConferenceIdentifier: ValidateConferenceIdentifier =
    fun (slug, accessCode) ->
        result {
            let! slug = slug |> UniqueSlug.create
            let! accessCode = accessCode |> AccessCode.create

            return (slug, accessCode)
        }

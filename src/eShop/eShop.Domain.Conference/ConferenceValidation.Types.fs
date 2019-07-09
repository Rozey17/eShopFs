namespace eShop.Domain.Conference

// ValidateConferenceIdentifier
type ValidateConferenceIdentifier = string * string -> Result<UniqueSlug * AccessCode, string>

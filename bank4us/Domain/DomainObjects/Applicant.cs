namespace Bank4Us.Domain.DomainObjects
{
    public enum IdentifierType
    {
        SSN,
        ITIN,
        Passport
    }

    public enum CitizenshipStatus
    {
        Citizen,
        PermanentResident,
        Unknown
    }


    public sealed record Applicant
    {
        public IdentifierType IdentifierType { get; init; } = IdentifierType.SSN;
        public string? IdentificationNumber { get; init; }
        public Address? Address { get; init; }
        public CitizenshipStatus CitizenshipStatus { get; init; } = CitizenshipStatus.Unknown;
        public decimal? OpeningDepositAmount { get; init; }
        public bool HasResidencyDocument { get; init; }
    }

}
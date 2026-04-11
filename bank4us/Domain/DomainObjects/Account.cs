namespace Bank4Us.Domain.DomainObjects
{
    public enum AccountType
    {
        Checking,
        Savings
    }

    public sealed record AccountHolder(string IdentificationNumber, IdentifierType IdentifierType);

    public sealed class Account
    {
        public Account() { }
        public int Id { get; set; }
        public AccountType AccountType { get; init; } = AccountType.Checking;

        public AccountHolder? AccountHolder { get; init; }

        public int Balance { get; set; } = 0;

    }
}

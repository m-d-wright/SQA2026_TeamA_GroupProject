namespace bank4us.Domain.DomainObjects
{
    public enum AccountType
    {
        Checking,
        Savings
    }
    // NOTE: This should probably be a contract for EF
    public sealed record AccountHolder(string Name, int Age);

    public sealed class Account
    {
        public AccountType AccountType { get; init; } = AccountType.Checking;

        public AccountHolder? AccountHolder { get; init; }

        public int Balance { get; set; } = 0;

    }
}

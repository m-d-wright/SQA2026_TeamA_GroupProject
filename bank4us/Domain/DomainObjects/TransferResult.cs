namespace bank4us.Domain.DomainObjects
{
    public sealed record TransferError(string message);

    public sealed record TransferResult(TransferStatus Status, IReadOnlyList<TransferError> Errors);
}

namespace bank4us.Domain.DomainObjects
{

    public enum ApplicationStatus
    {
        Approved,
        Cancelled,
        Incomplete,
        PendingVerification,
        NeedsExtraVerification
    }

    public sealed record ValidationError(string Message);

    public sealed record ProcessResult(ApplicationStatus Status, IReadOnlyList<ValidationError> Errors);
}

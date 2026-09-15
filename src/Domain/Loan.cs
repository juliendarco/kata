namespace LibraryManagement.Domain;

internal sealed class Loan
{
    public required Guid MemberId { get; init; }
    public required DateOnly BorrowedOn { get; init; }
    public required DateOnly DueOn { get; init; }
    
    public DateOnly? ReturnedOn { get; private set; }

    public bool IsActive => ReturnedOn is null;

    public void Return(DateOnly returnedOn)
    {
        if (ReturnedOn is not null)
        {
            throw new InvalidOperationException("Loan already returned.");
        }

        ReturnedOn = returnedOn;
    }

    public int DaysLate(DateOnly today)
    {
        var reference = ReturnedOn ?? today;
        return Math.Max(reference.DayNumber - DueOn.DayNumber, 0);
    }

    public decimal Penalty(DateOnly today) => LatePenaltyPolicy.Compute(DaysLate(today));
}
namespace LibraryManagement.Domain;

internal sealed class BookAvailability
{
    private readonly List<Loan> _loans = [];

    public required Book Book { get; init; }
    public required int TotalCopies { get; init; }

    public IReadOnlyList<Loan> Loans => _loans.ToArray();
    public int CopiesOnLoan => _loans.Count(x => x.IsActive);
    public int AvailableCopies => TotalCopies - CopiesOnLoan;
    public bool IsAvailable => AvailableCopies > 0;

    public bool HasActiveLoan(Guid memberId) => _loans.Any(x => x.IsActive && x.MemberId == memberId);

    public IEnumerable<Loan> LoansOf(Guid memberId) => _loans.Where(x => x.MemberId == memberId);

    public void Lend(MemberBase member, DateOnly today)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException($"No copy of {Book.Title} available.");
        }

        if (HasActiveLoan(member.Id))
        {
            throw new InvalidOperationException($"Member {member} already borrowed {Book.Title}.");
        }

        _loans.Add(new Loan
        {
            BorrowedOn = today,
            DueOn = today.AddDays(member.MaxLoanDurationInWeeks * 7),
            MemberId = member.Id,
        });
    }

    public int Return(MemberBase member, DateOnly today)
    {
        var loan = _loans.FirstOrDefault(x => x.IsActive && x.MemberId == member.Id)
                   ?? throw new InvalidOperationException($"Member {member} did not borrow {Book.Title}.");

        loan.Return(today);

        return loan.DaysLate(today);
    }
}
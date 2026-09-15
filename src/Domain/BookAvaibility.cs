namespace LibraryManagement.Domain;

internal sealed class BookAvailability
{
    private readonly List<Loan> _loans = [];
    
    public required Book Book { get; init; }
    public required int TotalCopies { get; init; }
    
    public IReadOnlyList<Loan> Loans => _loans.ToArray();
    public int CopiesOnLoan => _loans.Count;
    public int AvailableCopies => TotalCopies - CopiesOnLoan;
    public bool IsAvailable => AvailableCopies > 0;
    
    public bool HasActiveLoan(Guid memberId) => _loans.Any(x => x.MemberId == memberId);
    
    public void Lend(MemberBase member, TimeProvider timeProvider)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException($"No copy of {Book.Title} available.");
        }

        if (HasActiveLoan(member.Id))
        {
            throw new InvalidOperationException($"Member {member} already borrowed {Book.Title}.");
        }

        var today = timeProvider.GetToday();
        _loans.Add(new Loan(member.Id, today, today.AddDays(member.MaxLoanDurationInWeeks * 7)));
    }

    public void Return(MemberBase member)
    {
        var loan = _loans.FirstOrDefault(x => x.MemberId == member.Id)
                   ?? throw new InvalidOperationException($"Member {member} did not borrow {Book.Title}.");

        _loans.Remove(loan);
    }
}
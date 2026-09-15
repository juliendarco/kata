namespace LibraryManagement.Domain;

internal class BookAvailability
{
    private readonly List<Guid> _borrowerIds = [];
    
    public required Book Book { get; init; }
    public required int TotalCopies { get; init; }
    
    public IReadOnlyList<Guid> BorrowerIds => _borrowerIds.ToArray();
    public int CopiesOnLoan => _borrowerIds.Count;
    public int AvailableCopies => TotalCopies - CopiesOnLoan;
    public bool IsAvailable => AvailableCopies > 0;
    
    public void Lend(Guid memberId)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException($"No copy of {Book.Title} available.");
        }

        if (_borrowerIds.Contains(memberId))
        {
            throw new InvalidOperationException($"Member {memberId} already borrowed {Book.Title}.");
        }

        _borrowerIds.Add(memberId);
    }

    public void Return(Guid memberId)
    {
        if (!_borrowerIds.Remove(memberId))
        {
            throw new InvalidOperationException($"Member {memberId} did not borrow {Book.Title}.");
        }
    }
}
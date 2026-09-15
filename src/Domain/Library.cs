namespace LibraryManagement.Domain;

public class Library
{
    private readonly List<BookAvailability> _bookAvailabilities = [];
    private readonly List<MemberBase> _members = [];

    public IReadOnlyList<string> Books => _bookAvailabilities.Select(b => b.Book.Title).ToArray();
    public IReadOnlyList<MemberBase> Members => _members.ToArray();
    
    public Guid AddBook(string title, string author, int totalCopies = 1)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
        }
        
        if (_bookAvailabilities.Select(x => x.Book).Any(b => b.Title == title))
        {
            throw new InvalidOperationException($"Book {title} already exists.");
        }
        
        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(author));
        }

        if (totalCopies < 1)
        {
            throw new ArgumentException("Value must be greater than 0.", nameof(totalCopies));
        }

        var bookAvailability = new BookAvailability
        {
            Book = new Book
            {
                Title = title,
                Author = author
            },
            TotalCopies = totalCopies
        };
        _bookAvailabilities.Add(bookAvailability);
        
        return bookAvailability.Book.Id;
    }

    private MemberBase FindMember(Guid memberId)
        => _members.FirstOrDefault(m => m.Id == memberId)
           ?? throw new InvalidOperationException($"Member {memberId} is not registered.");
    
    private BookAvailability FindBook(Guid bookId)
        => _bookAvailabilities.FirstOrDefault(b => b.Book.Id == bookId)
           ?? throw new InvalidOperationException($"Book {bookId} does not exist.");
    
    public void BorrowBook(Guid bookId, Guid memberId, TimeProvider timeProvider)
    {
        var member = FindMember(memberId);
        var book = FindBook(bookId);

        var activeLoans = _bookAvailabilities.Count(b => b.HasActiveLoan(memberId));
        if (activeLoans >= member.MaxLoans)
        {
            throw new InvalidOperationException(
                $"Member {member} has reached the limit of {member.MaxLoans} simultaneous loans.");
        }

        book.Lend(member, timeProvider.GetToday());
    }
    
    public int ReturnBook(Guid bookId, Guid memberId, TimeProvider timeProvider)
    {
        var member = FindMember(memberId);
        var book = FindBook(bookId);

        return book.Return(member, timeProvider.GetToday());
    }
    
    public decimal GetTotalPenalties(Guid memberId, TimeProvider timeProvider)
    {
        FindMember(memberId);

        var today = timeProvider.GetToday();

        return _bookAvailabilities
            .SelectMany(b => b.LoansOf(memberId))
            .Sum(l => l.Penalty(today));
    }
    
    public void AddMember(MemberBase member)
    {
        if (_members.Any(m => m.Id == member.Id))
        {
            throw new InvalidOperationException($"Member {member} already exists.");
        }
        _members.Add(member);
    }

    public bool RemoveMember(MemberBase member)
    {
        return _members.Any(m => m.Id == member.Id) && _members.Remove(member);
    }
}
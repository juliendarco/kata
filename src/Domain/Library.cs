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

        var memberLoans = _bookAvailabilities.Count(b => b.HasActiveLoan(memberId));
        if (memberLoans >= member.MaxLoans)
        {
            throw new InvalidOperationException(
                $"Member {member} as reached the limit of {member.MaxLoans} simultaneous loans.");
        }

        if (!book.IsAvailable)
        {
            throw new InvalidOperationException($"Book {bookId} is not available.");
        }

        book.Lend(memberId, timeProvider);
    }
    
    public void ReturnBook(Guid bookId, Guid memberId)
    {
        var member = FindMember(memberId);
        var book = FindBook(bookId);
        
        
        book.Return(memberId, timeProvider);
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
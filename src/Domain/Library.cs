namespace LibraryManagement.Domain;

public class Library
{
    private readonly List<BookAvailability> _bookAvailabilities = [];
    private readonly List<MemberBase> _members = [];

    public IReadOnlyList<Book> Books => _bookAvailabilities.Select(b => b.Book).ToArray();
    public IReadOnlyList<MemberBase> Members => _members.ToArray();
    
    public Book AddBook(string title, string author, int totalCopies = 1)
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
        
        return bookAvailability.Book;
    }
    
    public void BorrowBook(Guid bookId, Guid memberId)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId)
                     ?? throw new InvalidOperationException($"Member {memberId} is not registered.");

        var availability = _bookAvailabilities.FirstOrDefault(b => b.Book.Id == bookId)
                           ?? throw new InvalidOperationException($"Book {bookId} does not exist.");

        var memberLoans = _bookAvailabilities.Count(b => b.BorrowerIds.Contains(memberId));
        if (memberLoans >= member.MaxLoans)
        {
            throw new InvalidOperationException(
                $"Member {member} as reached the limit of {member.MaxLoans} simultaneous loans.");
        }

        if (!availability.IsAvailable)
        {
            throw new InvalidOperationException($"Book {bookId} is not available.");
        }

        availability.Lend(memberId);
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
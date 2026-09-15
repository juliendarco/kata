namespace LibraryManagement.Domain;

public class Author
{
    private readonly List<Book> _books = [];
    
    public Guid Id { get; } = Guid.NewGuid();
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    
    public IReadOnlyList<Book> Books => _books;
    
    public override string ToString() => $"{FirstName} {LastName}";

    public Book AddBook(string title, int availableCopies = 1)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
        }
        
        if (_books.Any(b => b.Title == title))
        {
            throw new InvalidOperationException($"Book {title} already exists.");
        }

        if (availableCopies < 1)
        {
            throw new ArgumentException("Value must be greater than 0.", nameof(availableCopies));
        }

        var book = new Book
        {
            Title = title,
            AvailableCopies = availableCopies,
            Author = this
        };
        _books.Add(book);
        return book;
    }
}
namespace LibraryManagement.Domain.Tests;

public partial class LibraryTests
{
    private readonly Library _library = new();
    private readonly MemberFaker _memberFaker = new();
    private readonly StudentFaker _studentFaker = new();
    private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));

    private const string BookTitle = "Les Misérables";
    private const string BookAuthor = "Victor Hugo";
    
    private List<Guid> BorrowDistinctBooks(MemberBase member, int count)
    {
        var bookIds = new List<Guid>();

        for (var i = 0; i < count; i++)
        {
            var bookId = _library.AddBook($"Book {i}", $"Author {i}");
            _library.BorrowBook(bookId, member.Id, _timeProvider);
            bookIds.Add(bookId);
        }

        return bookIds;
    }
}
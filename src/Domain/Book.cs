namespace LibraryManagement.Domain;

public class Book
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Title { get; init; }
    public required Author Author { get; init; }
}
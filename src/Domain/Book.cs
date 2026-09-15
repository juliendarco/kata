namespace LibraryManagement.Domain;

public class Book
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Title { get; init; }
    public required string Author { get; init; }
    
    public override string ToString() => Title;
}
namespace LibraryManagement.Domain;

public class Author
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
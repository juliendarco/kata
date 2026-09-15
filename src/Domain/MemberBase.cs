namespace LibraryManagement.Domain;

public abstract class MemberBase
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    
    public override string ToString() => $"{FirstName} {LastName}";
    
    public abstract int MaxLoans { get; }
    public abstract int MaxLoanDurationInWeeks { get; }
}
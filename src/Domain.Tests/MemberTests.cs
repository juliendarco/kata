namespace LibraryManagement.Domain.Tests;

public class MemberTests
{
    private readonly MemberFaker _memberFakerFaker = new();
    
    [Fact]
    public void Should_HaveMaxLoans_Of5()
    {
        // Arrange & Act
        var member = _memberFakerFaker.Generate();
        
        // Assert
        member.MaxLoans.Should().Be(3);
    }
    
    [Fact]
    public void Should_HaveMaxLoanDurationInWeeks_Of4()
    {
        // Arrange & Act
        var member = _memberFakerFaker.Generate();
        
        // Assert
        member.MaxLoanDurationInWeeks.Should().Be(3);
    }
}

public sealed class MemberFaker : Faker<Member>
{
    public MemberFaker()
    {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
    }
}
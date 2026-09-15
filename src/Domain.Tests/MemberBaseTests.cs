namespace LibraryManagement.Domain.Tests;

public class MemberBaseTests
{
    private readonly MemberBaseFaker _memberBaseFaker = new();
    
    [Fact]
    public void Should_HaveId_WhenInitialized()
    {
        // Arrange & Act
        var member = _memberBaseFaker.Generate();
        
        // Assert
        member.Id.Should().NotBeEmpty();
    }
    
    [Fact]
    public void Should_HaveToString_ThatReturnsFirstNameAndLastName()
    {
        // Arrange & Act
        var person = _memberBaseFaker.Generate();
        
        // Assert
        person.ToString().Should().Be($"{person.FirstName} {person.LastName}");
    }
}

public sealed class MemberBaseForTest : MemberBase
{
    public override int MaxLoans => 2;
}

public sealed class MemberBaseFaker : Faker<MemberBaseForTest>
{
    public MemberBaseFaker()
    {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
    }
}
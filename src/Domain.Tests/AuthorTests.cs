namespace LibraryManagement.Domain.Tests;

public class AuthorTests
{
    private readonly AuthorFaker _authorFaker = new();
    
    [Fact]
    public void Should_HaveNoBooks_WhenCreated()
    {
        // Arrange & Act
        var author = _authorFaker.Generate();
        
        // Assert
        author.Books.Should().BeEmpty();
    }
    
    [Fact]
    public void Should_HaveToString_ThatReturnsFirstNameAndLastName()
    {
        // Arrange & Act
        var author = _authorFaker.Generate();
        
        // Assert
        author.ToString().Should().Be($"{author.FirstName} {author.LastName}");
    }
}

public sealed class AuthorFaker : Faker<Author>
{
    public AuthorFaker()
    {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
    }
}
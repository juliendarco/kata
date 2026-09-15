namespace LibraryManagement.Domain.Tests;

public partial class LibraryTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Throw_WhenTitleIsBlank(string title)
    {
        // Arrange & Act
        var act = () => _library.AddBook(title, BookAuthor);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(title));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Throw_WhenAuthorIsBlank(string author)
    {
        // Arrange & Act
        var act = () => _library.AddBook(BookTitle, author);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(author));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Throw_WhenTotalCopiesIsNotPositive(int totalCopies)
    {
        // Arrange & Act
        var act = () => _library.AddBook(BookTitle, BookAuthor, totalCopies);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(totalCopies));
    }

    [Fact]
    public void Should_Throw_WhenSameBookIsAddedTwice()
    {
        // Arrange
        _library.AddBook(BookTitle, BookAuthor);

        // Act
        var act = () => _library.AddBook(BookTitle, BookAuthor);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_AllowSameTitle_FromDifferentAuthors()
    {
        // Arrange
        _library.AddBook(BookTitle, BookAuthor);

        // Act
        _library.AddBook(BookTitle, "Jean Dupont");

        // Assert
        _library.Books.Should().HaveCount(2);
    }
}
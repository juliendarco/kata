namespace LibraryManagement.Domain.Tests;

public partial class LibraryTests
{
    [Fact]
    public void Should_AddMember()
    {
        // Arrange
        var member = _memberFaker.Generate();

        // Act
        _library.AddMember(member);

        // Assert
        _library.Members.Should().ContainSingle().Which.Should().Be(member);
    }

    [Fact]
    public void Should_Throw_WhenSameMemberIsAddedTwice()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);

        // Act
        var act = () => _library.AddMember(member);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_RemoveMember()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);

        // Act
        var removed = _library.RemoveMember(member);

        // Assert
        removed.Should().BeTrue();
        _library.Members.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotRemoveMember_WhenNotRegistered()
    {
        // Arrange & Act
        var removed = _library.RemoveMember(_memberFaker.Generate());

        // Assert
        removed.Should().BeFalse();
    }

    [Fact]
    public void Should_Throw_WhenRemovingMemberWithActiveLoans()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook("Les Misérables", "Victor Hugo");
        _library.BorrowBook(bookId, member.Id, _timeProvider);

        // Act
        var act = () => _library.RemoveMember(member);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_RemoveMember_AfterAllBooksAreReturned()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook("Les Misérables", "Victor Hugo");
        _library.BorrowBook(bookId, member.Id, _timeProvider);
        _library.ReturnBook(bookId, member.Id, _timeProvider);

        // Act
        var removed = _library.RemoveMember(member);

        // Assert
        removed.Should().BeTrue();
    }
}
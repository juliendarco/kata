namespace LibraryManagement.Domain.Tests;

public partial class LibraryTests
{
    [Fact]
    public void Should_Throw_WhenBorrowingMemberIsNotRegistered()
    {
        // Arrange
        var bookId = _library.AddBook(BookTitle, BookAuthor);

        // Act
        var act = () => _library.BorrowBook(bookId, Guid.NewGuid(), _timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenBorrowingUnknownBook()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);

        // Act
        var act = () => _library.BorrowBook(Guid.NewGuid(), member.Id, _timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenNoCopyIsAvailable()
    {
        // Arrange
        var firstMember = _memberFaker.Generate();
        var secondMember = _memberFaker.Generate();
        _library.AddMember(firstMember);
        _library.AddMember(secondMember);
        var bookId = _library.AddBook(BookTitle, BookAuthor);
        _library.BorrowBook(bookId, firstMember.Id, _timeProvider);

        // Act
        var act = () => _library.BorrowBook(bookId, secondMember.Id, _timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenMemberReachedLoanLimit()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        BorrowDistinctBooks(member, member.MaxLoans);
        var extraBookId = _library.AddBook("Extra", "Author");

        // Act
        var act = () => _library.BorrowBook(extraBookId, member.Id, _timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"*{member.MaxLoans}*");
    }

    [Fact]
    public void Should_AllowStudentToBorrowMoreThanStandardMember()
    {
        // Arrange
        var student = _studentFaker.Generate();
        _library.AddMember(student);

        // Act
        var act = () => BorrowDistinctBooks(student, student.MaxLoans);

        // Assert
        act.Should().NotThrow();
        student.MaxLoans.Should().BeGreaterThan(_memberFaker.Generate().MaxLoans);
    }

    [Fact]
    public void Should_AllowBorrowingAgain_AfterReturningAtTheLimit()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var borrowedIds = BorrowDistinctBooks(member, member.MaxLoans);
        _library.ReturnBook(borrowedIds[0], member.Id, _timeProvider);
        var extraBookId = _library.AddBook("Extra", "Author");

        // Act
        var act = () => _library.BorrowBook(extraBookId, member.Id, _timeProvider);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_ReturnNoDaysLate_WhenReturnedOnTime()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook(BookTitle, BookAuthor);
        _library.BorrowBook(bookId, member.Id, _timeProvider);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7));

        // Act
        var daysLate = _library.ReturnBook(bookId, member.Id, _timeProvider);

        // Assert
        daysLate.Should().Be(0);
    }

    [Fact]
    public void Should_ReturnDaysLate_WhenReturnedLate()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook(BookTitle, BookAuthor);
        _library.BorrowBook(bookId, member.Id, _timeProvider);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7 + 5));

        // Act
        var daysLate = _library.ReturnBook(bookId, member.Id, _timeProvider);

        // Assert
        daysLate.Should().Be(5);
    }

    [Fact]
    public void Should_Throw_WhenReturningBookNotBorrowed()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook(BookTitle, BookAuthor);

        // Act
        var act = () => _library.ReturnBook(bookId, member.Id, _timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
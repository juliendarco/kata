namespace LibraryManagement.Domain.Tests;

public partial class LibraryTests
{
    [Fact]
    public void Should_HaveNoPenalties_WhenMemberNeverBorrowed()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);

        // Act
        var penalties = _library.GetTotalPenalties(member.Id, _timeProvider);

        // Assert
        penalties.Should().Be(0m);
    }

    [Fact]
    public void Should_HaveNoPenalties_WhenLoansAreStillWithinDueDate()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook(BookTitle, BookAuthor);
        _library.BorrowBook(bookId, member.Id, _timeProvider);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7));

        // Act
        var penalties = _library.GetTotalPenalties(member.Id, _timeProvider);

        // Assert
        penalties.Should().Be(0m);
    }

    [Fact]
    public void Should_ComputePenalties_ForLoanStillNotReturned()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook(BookTitle, BookAuthor);
        _library.BorrowBook(bookId, member.Id, _timeProvider);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7 + 5));

        // Act
        var penalties = _library.GetTotalPenalties(member.Id, _timeProvider);

        // Assert
        penalties.Should().Be(1.00m);
    }

    [Fact]
    public void Should_KeepPenalties_AfterBookIsReturnedLate()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        var bookId = _library.AddBook(BookTitle, BookAuthor);
        _library.BorrowBook(bookId, member.Id, _timeProvider);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7 + 5));
        _library.ReturnBook(bookId, member.Id, _timeProvider);
        _timeProvider.Advance(TimeSpan.FromDays(100));

        // Act
        var penalties = _library.GetTotalPenalties(member.Id, _timeProvider);

        // Assert
        penalties.Should().Be(1.00m);
    }

    [Fact]
    public void Should_SumPenalties_AcrossSeveralBooks()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        BorrowDistinctBooks(member, 2);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7 + 3));

        // Act
        var penalties = _library.GetTotalPenalties(member.Id, _timeProvider);

        // Assert
        penalties.Should().Be(1.20m);
    }

    [Fact]
    public void Should_CapPenalties_PerLoan()
    {
        // Arrange
        var member = _memberFaker.Generate();
        _library.AddMember(member);
        BorrowDistinctBooks(member, 2);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7 + 500));

        // Act
        var penalties = _library.GetTotalPenalties(member.Id, _timeProvider);

        // Assert
        penalties.Should().Be(LatePenaltyPolicy.MaxPenalty * 2);
    }

    [Fact]
    public void Should_NotCountOtherMembersPenalties()
    {
        // Arrange
        var member = _memberFaker.Generate();
        var otherMember = _memberFaker.Generate();
        _library.AddMember(member);
        _library.AddMember(otherMember);
        var bookId = _library.AddBook(BookTitle, BookAuthor, 2);
        _library.BorrowBook(bookId, otherMember.Id, _timeProvider);
        _timeProvider.Advance(TimeSpan.FromDays(member.MaxLoanDurationInWeeks * 7 + 5));

        // Act
        var penalties = _library.GetTotalPenalties(member.Id, _timeProvider);

        // Assert
        penalties.Should().Be(0m);
    }

    [Fact]
    public void Should_Throw_WhenComputingPenaltiesForUnknownMember()
    {
        // Arrange & Act
        var act = () => _library.GetTotalPenalties(Guid.NewGuid(), _timeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
namespace LibraryManagement.Domain.Tests;

public class BookAvailabilityTests
{
    private static readonly DateOnly FakeDate = new(2026, 1, 15);

    private readonly BookAvailabilityFaker _bookAvailabilityFaker = new();
    private readonly FakeTimeProvider _fakeTimeProvider = new(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));

    [Fact]
    public void Should_HaveAllCopiesAvailable_WhenCreated()
    {
        // Arrange & Act
        var availability = _bookAvailabilityFaker.WithTotalCopies(3).Generate();

        // Assert
        availability.CopiesOnLoan.Should().Be(0);
        availability.AvailableCopies.Should().Be(3);
        availability.IsAvailable.Should().BeTrue();
        availability.Loans.Should().BeEmpty();
    }

    [Fact]
    public void Should_DecreaseAvailableCopies_WhenLent()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();

        // Act
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Assert
        availability.AvailableCopies.Should().Be(1);
        availability.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Should_TrackBorrower_WhenLent()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();

        // Act
        availability.Lend(memberId, _fakeTimeProvider);

        // Assert
        availability.CopiesOnLoan.Should().Be(1);
        availability.Loans.Should().ContainSingle().Which.MemberId.Should().Be(memberId);
    }

    [Fact]
    public void Should_RecordBorrowedOnDate_WhenLent()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();

        // Act
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Assert
        availability.Loans.Should().ContainSingle().Which.BorrowedOn.Should().Be(FakeDate);
    }

    [Fact]
    public void Should_NotBeAvailable_WhenAllCopiesAreLent()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();

        // Act
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Assert
        availability.AvailableCopies.Should().Be(0);
        availability.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void Should_Throw_WhenNoCopyAvailable()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(1).Generate();
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Act
        var act = () => availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenSameMemberBorrowsTwice()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId, _fakeTimeProvider);

        // Act
        var act = () => availability.Lend(memberId, _fakeTimeProvider);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_HaveActiveLoan_WhenMemberBorrowed()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId, _fakeTimeProvider);

        // Act
        var hasActiveLoan = availability.HasActiveLoan(memberId);

        // Assert
        hasActiveLoan.Should().BeTrue();
    }

    [Fact]
    public void Should_HaveNoActiveLoan_WhenMemberNeverBorrowed()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Act
        var hasActiveLoan = availability.HasActiveLoan(Guid.NewGuid());

        // Assert
        hasActiveLoan.Should().BeFalse();
    }

    [Fact]
    public void Should_HaveNoActiveLoan_WhenMemberReturned()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId, _fakeTimeProvider);
        availability.Return(memberId);

        // Act
        var hasActiveLoan = availability.HasActiveLoan(memberId);

        // Assert
        hasActiveLoan.Should().BeFalse();
    }

    [Fact]
    public void Should_RestoreAvailableCopies_WhenReturned()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId, _fakeTimeProvider);

        // Act
        availability.Return(memberId);

        // Assert
        availability.CopiesOnLoan.Should().Be(0);
        availability.AvailableCopies.Should().Be(2);
        availability.Loans.Should().BeEmpty();
    }

    [Fact]
    public void Should_BeAvailableAgain_WhenLastCopyIsReturned()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(1).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId, _fakeTimeProvider);

        // Act
        availability.Return(memberId);

        // Assert
        availability.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Should_AllowBorrowingAgain_AfterReturn()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(1).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId, _fakeTimeProvider);
        availability.Return(memberId);

        // Act
        availability.Lend(memberId, _fakeTimeProvider);

        // Assert
        availability.CopiesOnLoan.Should().Be(1);
    }

    [Fact]
    public void Should_Throw_WhenReturningBookNotBorrowed()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();

        // Act
        var act = () => availability.Return(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenReturningTwice()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId, _fakeTimeProvider);
        availability.Return(memberId);

        // Act
        var act = () => availability.Return(memberId);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_NotAffectOtherBorrowers_WhenOneReturns()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(3).Generate();
        var firstMemberId = Guid.NewGuid();
        var secondMemberId = Guid.NewGuid();
        availability.Lend(firstMemberId, _fakeTimeProvider);
        availability.Lend(secondMemberId, _fakeTimeProvider);

        // Act
        availability.Return(firstMemberId);

        // Assert
        availability.Loans.Should().ContainSingle().Which.MemberId.Should().Be(secondMemberId);
    }

    [Fact]
    public void Should_NotExposeInternalList_ThroughLoans()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Act
        var loans = availability.Loans;
        availability.Lend(Guid.NewGuid(), _fakeTimeProvider);

        // Assert
        loans.Should().ContainSingle();
    }
}

internal sealed class BookAvailabilityFaker : Faker<BookAvailability>
{
    public BookAvailabilityFaker() => WithTotalCopies(null);

    public BookAvailabilityFaker WithTotalCopies(int totalCopies) => WithTotalCopies((int?)totalCopies);

    private BookAvailabilityFaker WithTotalCopies(int? totalCopies)
    {
        CustomInstantiator(f => new BookAvailability
        {
            Book = new Book
            {
                Title = f.Commerce.ProductName(),
                Author = f.Name.FullName()
            },
            TotalCopies = totalCopies ?? f.Random.Int(1, 10)
        });

        return this;
    }
}
namespace LibraryManagement.Domain.Tests;

public class BookAvailabilityTests
{
    private static readonly DateOnly Today = new(2026, 1, 15);

    private readonly BookAvailabilityFaker _bookAvailabilityFaker = new();
    private readonly MemberFaker _memberFaker = new();

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
        availability.Lend(_memberFaker.Generate(), Today);

        // Assert
        availability.AvailableCopies.Should().Be(1);
        availability.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Should_TrackBorrower_WhenLent()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();

        // Act
        availability.Lend(member, Today);

        // Assert
        availability.CopiesOnLoan.Should().Be(1);
        availability.Loans.Should().ContainSingle().Which.MemberId.Should().Be(member.Id);
    }

    [Fact]
    public void Should_RecordBorrowedOnDate_WhenLent()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();

        // Act
        availability.Lend(_memberFaker.Generate(), Today);

        // Assert
        availability.Loans.Should().ContainSingle().Which.BorrowedOn.Should().Be(Today);
    }

    [Fact]
    public void Should_ComputeDueDate_FromMemberLoanDuration()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();

        // Act
        availability.Lend(member, Today);

        // Assert
        availability.Loans.Should().ContainSingle()
            .Which.DueOn.Should().Be(Today.AddDays(member.MaxLoanDurationInWeeks * 7));
    }

    [Fact]
    public void Should_NotBeAvailable_WhenAllCopiesAreLent()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();

        // Act
        availability.Lend(_memberFaker.Generate(), Today);
        availability.Lend(_memberFaker.Generate(), Today);

        // Assert
        availability.AvailableCopies.Should().Be(0);
        availability.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void Should_Throw_WhenNoCopyAvailable()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(1).Generate();
        availability.Lend(_memberFaker.Generate(), Today);

        // Act
        var act = () => availability.Lend(_memberFaker.Generate(), Today);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenSameMemberBorrowsTwice()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);

        // Act
        var act = () => availability.Lend(member, Today);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_HaveActiveLoan_WhenMemberBorrowed()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);

        // Act
        var hasActiveLoan = availability.HasActiveLoan(member.Id);

        // Assert
        hasActiveLoan.Should().BeTrue();
    }

    [Fact]
    public void Should_HaveNoActiveLoan_WhenMemberNeverBorrowed()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        availability.Lend(_memberFaker.Generate(), Today);

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
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);
        availability.Return(member, Today);

        // Act
        var hasActiveLoan = availability.HasActiveLoan(member.Id);

        // Assert
        hasActiveLoan.Should().BeFalse();
    }

    [Fact]
    public void Should_RestoreAvailableCopies_WhenReturned()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);

        // Act
        availability.Return(member, Today);

        // Assert
        availability.CopiesOnLoan.Should().Be(0);
        availability.AvailableCopies.Should().Be(2);
    }

    [Fact]
    public void Should_KeepLoanHistory_WhenReturned()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);

        // Act
        availability.Return(member, Today);

        // Assert
        availability.Loans.Should().ContainSingle().Which.ReturnedOn.Should().Be(Today);
    }

    [Fact]
    public void Should_ReturnNoDaysLate_WhenReturnedOnTime()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);

        // Act
        var daysLate = availability.Return(member, Today.AddDays(member.MaxLoanDurationInWeeks * 7));

        // Assert
        daysLate.Should().Be(0);
    }

    [Fact]
    public void Should_ReturnDaysLate_WhenReturnedLate()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);

        // Act
        var daysLate = availability.Return(member, Today.AddDays(member.MaxLoanDurationInWeeks * 7 + 5));

        // Assert
        daysLate.Should().Be(5);
    }

    [Fact]
    public void Should_BeAvailableAgain_WhenLastCopyIsReturned()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(1).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);

        // Act
        availability.Return(member, Today);

        // Assert
        availability.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Should_AllowBorrowingAgain_AfterReturn()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(1).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);
        availability.Return(member, Today);

        // Act
        availability.Lend(member, Today);

        // Assert
        availability.CopiesOnLoan.Should().Be(1);
        availability.Loans.Should().HaveCount(2);
    }

    [Fact]
    public void Should_Throw_WhenReturningBookNotBorrowed()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();

        // Act
        var act = () => availability.Return(_memberFaker.Generate(), Today);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenReturningTwice()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);
        availability.Return(member, Today);

        // Act
        var act = () => availability.Return(member, Today);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_NotAffectOtherBorrowers_WhenOneReturns()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(3).Generate();
        var firstMember = _memberFaker.Generate();
        var secondMember = _memberFaker.Generate();
        availability.Lend(firstMember, Today);
        availability.Lend(secondMember, Today);

        // Act
        availability.Return(firstMember, Today);

        // Assert
        availability.CopiesOnLoan.Should().Be(1);
        availability.HasActiveLoan(secondMember.Id).Should().BeTrue();
    }

    [Fact]
    public void Should_ReturnOnlyMemberLoans_WhenListingLoansOf()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(3).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);
        availability.Lend(_memberFaker.Generate(), Today);

        // Act
        var loans = availability.LoansOf(member.Id);

        // Assert
        loans.Should().ContainSingle().Which.MemberId.Should().Be(member.Id);
    }

    [Fact]
    public void Should_IncludeReturnedLoans_WhenListingLoansOf()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        var member = _memberFaker.Generate();
        availability.Lend(member, Today);
        availability.Return(member, Today);

        // Act
        var loans = availability.LoansOf(member.Id);

        // Assert
        loans.Should().ContainSingle().Which.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Should_NotExposeInternalList_ThroughLoans()
    {
        // Arrange
        var availability = _bookAvailabilityFaker.WithTotalCopies(2).Generate();
        availability.Lend(_memberFaker.Generate(), Today);

        // Act
        var loans = availability.Loans;
        availability.Lend(_memberFaker.Generate(), Today);

        // Assert
        loans.Should().ContainSingle();
    }
}

internal sealed class BookAvailabilityFaker : Faker<BookAvailability>
{
    private int? _totalCopies;

    public BookAvailabilityFaker()
        => CustomInstantiator(f => new BookAvailability
        {
            Book = new Book
            {
                Title = f.Commerce.ProductName(),
                Author = f.Name.FullName()
            },
            TotalCopies = _totalCopies ?? f.Random.Int(1, 10)
        });

    public BookAvailabilityFaker WithTotalCopies(int totalCopies)
    {
        _totalCopies = totalCopies;
        return this;
    }
}
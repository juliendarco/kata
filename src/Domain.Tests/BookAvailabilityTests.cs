namespace LibraryManagement.Domain.Tests;

public class BookAvailabilityTests
{
    [Fact]
    public void Should_HaveAllCopiesAvailable_WhenCreated()
    {
        // Arrange & Act
        var availability = new BookAvailabilityFaker().WithTotalCopies(3).Generate();

        // Assert
        availability.CopiesOnLoan.Should().Be(0);
        availability.AvailableCopies.Should().Be(3);
        availability.IsAvailable.Should().BeTrue();
        availability.BorrowerIds.Should().BeEmpty();
    }

    [Fact]
    public void Should_DecreaseAvailableCopies_WhenLent()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();

        // Act
        availability.Lend(Guid.NewGuid());

        // Assert
        availability.AvailableCopies.Should().Be(1);
        availability.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Should_TrackBorrower_WhenLent()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();

        // Act
        availability.Lend(memberId);

        // Assert
        availability.CopiesOnLoan.Should().Be(1);
        availability.BorrowerIds.Should().ContainSingle().Which.Should().Be(memberId);
    }

    [Fact]
    public void Should_NotBeAvailable_WhenAllCopiesAreLent()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();

        // Act
        availability.Lend(Guid.NewGuid());
        availability.Lend(Guid.NewGuid());

        // Assert
        availability.AvailableCopies.Should().Be(0);
        availability.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void Should_Throw_WhenNoCopyAvailable()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(1).Generate();
        availability.Lend(Guid.NewGuid());

        // Act
        var act = () => availability.Lend(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenSameMemberBorrowsTwice()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId);

        // Act
        var act = () => availability.Lend(memberId);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_RestoreAvailableCopies_WhenReturned()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId);

        // Act
        availability.Return(memberId);

        // Assert
        availability.CopiesOnLoan.Should().Be(0);
        availability.AvailableCopies.Should().Be(2);
        availability.BorrowerIds.Should().BeEmpty();
    }

    [Fact]
    public void Should_BeAvailableAgain_WhenLastCopyIsReturned()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(1).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId);

        // Act
        availability.Return(memberId);

        // Assert
        availability.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Should_AllowBorrowingAgain_AfterReturn()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(1).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId);
        availability.Return(memberId);

        // Act
        availability.Lend(memberId);

        // Assert
        availability.CopiesOnLoan.Should().Be(1);
    }

    [Fact]
    public void Should_Throw_WhenReturningBookNotBorrowed()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();

        // Act
        var act = () => availability.Return(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_Throw_WhenReturningTwice()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();
        var memberId = Guid.NewGuid();
        availability.Lend(memberId);
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
        var availability = new BookAvailabilityFaker().WithTotalCopies(3).Generate();
        var firstMemberId = Guid.NewGuid();
        var secondMemberId = Guid.NewGuid();
        availability.Lend(firstMemberId);
        availability.Lend(secondMemberId);

        // Act
        availability.Return(firstMemberId);

        // Assert
        availability.BorrowerIds.Should().ContainSingle().Which.Should().Be(secondMemberId);
    }

    [Fact]
    public void Should_NotExposeInternalList_ThroughBorrowerIds()
    {
        // Arrange
        var availability = new BookAvailabilityFaker().WithTotalCopies(2).Generate();
        availability.Lend(Guid.NewGuid());

        // Act
        var borrowerIds = availability.BorrowerIds;
        availability.Lend(Guid.NewGuid());

        // Assert
        borrowerIds.Should().ContainSingle();
    }
}

public sealed class BookAvailabilityFaker : Faker<BookAvailability>
{
    public BookAvailabilityFaker()
    {
        CustomInstantiator(f => new BookAvailability
        {
            Book = new Book
            {
                Title = f.Commerce.ProductName(),
                Author = f.Name.FullName()
            },
            TotalCopies = f.Random.Int(1, 10)
        });
    }

    public BookAvailabilityFaker WithTotalCopies(int totalCopies)
    {
        CustomInstantiator(f => new BookAvailability
        {
            Book = new Book
            {
                Title = f.Commerce.ProductName(),
                Author = f.Name.FullName()
            },
            TotalCopies = totalCopies
        });

        return this;
    }
}
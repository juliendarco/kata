namespace LibraryManagement.Domain.Tests;

public class LoanTests
{
    private static readonly DateOnly DueOn = new(2026, 2, 5);

    private readonly LoanFaker _loanFaker = new();

    [Fact]
    public void Should_BeActive_WhenCreated()
    {
        // Arrange & Act
        var loan = _loanFaker.Generate();

        // Assert
        loan.IsActive.Should().BeTrue();
        loan.ReturnedOn.Should().BeNull();
    }

    [Fact]
    public void Should_NotBeActive_WhenReturned()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act
        loan.Return(DueOn);

        // Assert
        loan.IsActive.Should().BeFalse();
        loan.ReturnedOn.Should().Be(DueOn);
    }

    [Fact]
    public void Should_Throw_WhenReturnedTwice()
    {
        // Arrange
        var loan = _loanFaker.Generate();
        loan.Return(DueOn);

        // Act
        var act = () => loan.Return(DueOn);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Should_HaveNoDaysLate_WhenStillWithinDueDate()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act
        var daysLate = loan.DaysLate(DueOn.AddDays(-1));

        // Assert
        daysLate.Should().Be(0);
    }

    [Fact]
    public void Should_HaveNoDaysLate_OnDueDate()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act
        var daysLate = loan.DaysLate(DueOn);

        // Assert
        daysLate.Should().Be(0);
    }

    [Fact]
    public void Should_CountDaysLate_WhenStillNotReturned()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act
        var daysLate = loan.DaysLate(DueOn.AddDays(3));

        // Assert
        daysLate.Should().Be(3);
    }

    [Fact]
    public void Should_KeepCountingDaysLate_AsTimePasses()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act & Assert
        loan.DaysLate(DueOn.AddDays(1)).Should().Be(1);
        loan.DaysLate(DueOn.AddDays(10)).Should().Be(10);
    }

    [Fact]
    public void Should_HaveNoDaysLate_WhenReturnedOnTime()
    {
        // Arrange
        var loan = _loanFaker.Generate();
        loan.Return(DueOn);

        // Act
        var daysLate = loan.DaysLate(DueOn.AddDays(30));

        // Assert
        daysLate.Should().Be(0);
    }

    [Fact]
    public void Should_FreezeDaysLate_WhenReturnedLate()
    {
        // Arrange
        var loan = _loanFaker.Generate();
        loan.Return(DueOn.AddDays(4));

        // Act
        var daysLate = loan.DaysLate(DueOn.AddDays(30));

        // Assert
        daysLate.Should().Be(4);
    }

    [Fact]
    public void Should_HaveNoPenalty_WhenNotLate()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act
        var penalty = loan.Penalty(DueOn);

        // Assert
        penalty.Should().Be(0m);
    }

    [Fact]
    public void Should_ComputePenalty_WhenLate()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act
        var penalty = loan.Penalty(DueOn.AddDays(3));

        // Assert
        penalty.Should().Be(0.60m);
    }

    [Fact]
    public void Should_CapPenalty_WhenVeryLate()
    {
        // Arrange
        var loan = _loanFaker.Generate();

        // Act
        var penalty = loan.Penalty(DueOn.AddDays(200));

        // Assert
        penalty.Should().Be(LatePenaltyPolicy.MaxPenalty);
    }
}

internal sealed class LoanFaker : Faker<Loan>
{
    private readonly DateOnly _borrowedOn = new(2026, 1, 15);
    private readonly DateOnly _dueOn = new(2026, 2, 5);

    public LoanFaker()
        => CustomInstantiator(f => new Loan
        {
            MemberId = f.Random.Guid(),
            BorrowedOn = _borrowedOn,
            DueOn = _dueOn
        });
}
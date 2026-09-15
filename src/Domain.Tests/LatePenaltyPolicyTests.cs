namespace LibraryManagement.Domain.Tests;

public class LatePenaltyPolicyTests
{
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 0.20)]
    [InlineData(3, 0.60)]
    [InlineData(49, 9.80)]
    [InlineData(50, 10)]
    [InlineData(100, 10)]
    public void Should_ComputePenalty(int daysLate, decimal expected)
        => LatePenaltyPolicy.Compute(daysLate).Should().Be(expected);
}
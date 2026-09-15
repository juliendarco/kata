namespace LibraryManagement.Domain.Tests;

public class StudentTests
{
    private readonly StudentFaker _studentFaker = new();
    
    [Fact]
    public void Should_HaveMaxLoans_Of5()
    {
        // Arrange & Act
        var student = _studentFaker.Generate();
        
        // Assert
        student.MaxLoans.Should().Be(5);
    }
    
    [Fact]
    public void Should_HaveMaxLoanDurationInWeeks_Of4()
    {
        // Arrange & Act
        var student = _studentFaker.Generate();
        
        // Assert
        student.MaxLoanDurationInWeeks.Should().Be(4);
    }
}

public sealed class StudentFaker : Faker<Student>
{
    public StudentFaker()
    {
        RuleFor(a => a.FirstName, f => f.Name.FirstName());
        RuleFor(a => a.LastName, f => f.Name.LastName());
    }
}
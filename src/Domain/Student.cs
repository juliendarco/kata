namespace LibraryManagement.Domain;

public class Student : MemberBase
{
    public override int MaxLoans => 5;
    public override int MaxLoanDurationInWeeks => 4;
}
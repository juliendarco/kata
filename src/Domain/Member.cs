namespace LibraryManagement.Domain;

public class Member : MemberBase
{
    public override int MaxLoans => 3;
    public override int MaxLoanDurationInWeeks => 3;
}
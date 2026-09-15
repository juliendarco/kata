namespace LibraryManagement.Domain;

public class Member : MemberBase
{
    public override int MaxLoans => 3;
}
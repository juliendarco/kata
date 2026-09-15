namespace LibraryManagement.Domain;

public class Student : MemberBase
{
    public override int MaxLoans => 5;
}
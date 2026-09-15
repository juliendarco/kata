namespace LibraryManagement.Domain;

internal record Loan(Guid MemberId, DateOnly BorrowedOn, DateOnly DueDate);
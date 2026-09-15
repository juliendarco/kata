namespace LibraryManagement.Domain;

internal static class LatePenaltyPolicy
{
    internal const decimal PenaltyPerDay = 0.20m;
    internal const decimal MaxPenalty = 10m;

    public static decimal Compute(int daysLate)
        => daysLate <= 0 ? 0m : Math.Min(daysLate * PenaltyPerDay, MaxPenalty);
}
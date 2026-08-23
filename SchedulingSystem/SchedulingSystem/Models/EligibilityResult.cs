namespace SchedulingSystem.Models;

/// <summary>Result of checking whether a student may be assigned to a course.</summary>
public record EligibilityResult(bool IsEligible, IReadOnlyList<string> Reasons)
{
    public static readonly EligibilityResult Eligible = new(true, []);
}

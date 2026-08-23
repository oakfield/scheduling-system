namespace SchedulingSystem.Models;

/// <summary>Result of attempting to assign a student to a course.</summary>
public record AssignmentResult(bool Success, IReadOnlyList<string> Reasons)
{
    public static readonly AssignmentResult Ok = new(true, []);

    public static AssignmentResult Failed(IReadOnlyList<string> reasons) => new(false, reasons);

    public static AssignmentResult Failed(string reason) => new(false, [reason]);
}

namespace SchedulingSystem.Models;

/// <summary>
/// Everything needed to evaluate and display a student's eligibility for courses in the
/// upcoming semester. Separate from <see cref="StudentListItem"/> (used by the read-only
/// Students page) because this also needs the student's in-progress assignments for the
/// upcoming term, not just their completed-course history.
/// </summary>
public record StudentAssignmentProfile(
    int Id,
    string FullName,
    string Email,
    int YearLevel,
    int CurrentCredits,
    int MaxCreditsAllowed,
    IReadOnlyList<string> CompletedCourseNumbers,
    IReadOnlyList<string> AssignedCourseNumbers,
    int AssignedCreditsThisTerm);

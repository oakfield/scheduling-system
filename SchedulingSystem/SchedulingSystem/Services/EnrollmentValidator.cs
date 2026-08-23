using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

/// <summary>
/// Pure eligibility rules for assigning a student to a course in the upcoming semester.
/// No I/O, so it can be exercised the same way whether it's annotating the catalog table
/// for display or gating the actual write in <see cref="IEnrollmentService"/>.
/// </summary>
public static class EnrollmentValidator
{
    public static EligibilityResult Evaluate(StudentAssignmentProfile student, CourseListItem course)
    {
        var reasons = new List<string>();

        if (Contains(student.AssignedCourseNumbers, course.CourseNumber))
        {
            reasons.Add("Already assigned to this course for the upcoming semester.");
        }

        // A course assigned this term (not just previously completed) also satisfies
        // prerequisites for other courses assigned in the same session.
        var satisfied = new HashSet<string>(student.CompletedCourseNumbers, StringComparer.OrdinalIgnoreCase);
        satisfied.UnionWith(student.AssignedCourseNumbers);

        var missingPrerequisites = course.PrerequisiteCourseNumbers
            .Where(p => !satisfied.Contains(p))
            .ToList();
        if (missingPrerequisites.Count > 0)
        {
            reasons.Add($"Missing prerequisite(s): {string.Join(", ", missingPrerequisites)}.");
        }

        if (course.AvailableSeats <= 0)
        {
            reasons.Add($"No seats available (0 of {course.Capacity}).");
        }

        var creditsAfterAssignment = student.AssignedCreditsThisTerm + course.Credits;
        if (creditsAfterAssignment > student.MaxCreditsAllowed)
        {
            reasons.Add(
                $"Would exceed max credits ({creditsAfterAssignment} of {student.MaxCreditsAllowed} allowed).");
        }

        return reasons.Count == 0 ? EligibilityResult.Eligible : new EligibilityResult(false, reasons);
    }

    private static bool Contains(IReadOnlyList<string> values, string value) =>
        values.Contains(value, StringComparer.OrdinalIgnoreCase);
}

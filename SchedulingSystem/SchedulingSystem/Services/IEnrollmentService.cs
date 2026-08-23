using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

public interface IEnrollmentService
{
    /// <summary>
    /// Assigns a student to a course for the upcoming semester, after re-checking
    /// eligibility against freshly-loaded data (not whatever the caller had cached) so a
    /// stale UI can't bypass the same rules <see cref="EnrollmentValidator"/> shows on
    /// screen - e.g. a seat taken by another registrar between page load and this call.
    /// </summary>
    Task<AssignmentResult> AssignStudentToCourseAsync(
        int studentId,
        int courseId,
        CancellationToken cancellationToken = default);
}

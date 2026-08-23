using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

public interface ICourseService
{
    /// <summary>
    /// Returns every course in the catalog. The result set is small enough (a single
    /// semester's course catalog in this exercise) that callers filter it in memory
    /// rather than pushing filter criteria down to the database.
    /// </summary>
    Task<IReadOnlyList<CourseListItem>> GetCoursesAsync(CancellationToken cancellationToken = default);
}

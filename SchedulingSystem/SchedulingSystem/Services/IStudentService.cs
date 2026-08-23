using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

public interface IStudentService
{
    /// <summary>
    /// Returns every student with their academic status. The result set is small enough
    /// (a university semester's worth of students in this exercise) that callers filter
    /// it in memory rather than pushing filter criteria down to the database.
    /// </summary>
    Task<IReadOnlyList<StudentListItem>> GetStudentsAsync(CancellationToken cancellationToken = default);
}

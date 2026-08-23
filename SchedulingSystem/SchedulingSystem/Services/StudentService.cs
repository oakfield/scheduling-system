using Microsoft.EntityFrameworkCore;
using SchedulingSystem.Data;
using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

public class StudentService(IDbContextFactory<SchedulingDbContext> contextFactory) : IStudentService
{
    public async Task<IReadOnlyList<StudentListItem>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var students = await db.Students
            .AsNoTracking()
            .Include(s => s.CompletedCourses)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync(cancellationToken);

        return students.Select(EntityMappings.ToListItem).ToList();
    }

    public async Task<StudentAssignmentProfile?> GetStudentProfileAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var student = await db.Students
            .AsNoTracking()
            .Include(s => s.CompletedCourses)
            .Include(s => s.AssignedCourses)
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);

        return student is null ? null : EntityMappings.ToAssignmentProfile(student);
    }
}

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

        return students
            .Select(s => new StudentListItem(
                s.Id,
                s.FirstName,
                s.LastName,
                s.Email,
                s.HouseAffiliation,
                s.YearLevel,
                s.CurrentCredits,
                s.MaxCreditsAllowed,
                s.CompletedCourses
                    .Select(c => c.CourseNumber)
                    .OrderBy(c => c)
                    .ToList()))
            .ToList();
    }
}

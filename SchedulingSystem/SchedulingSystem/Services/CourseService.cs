using Microsoft.EntityFrameworkCore;
using SchedulingSystem.Data;
using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

public class CourseService(IDbContextFactory<SchedulingDbContext> contextFactory) : ICourseService
{
    public async Task<IReadOnlyList<CourseListItem>> GetCoursesAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var courses = await db.Courses
            .AsNoTracking()
            .Include(c => c.Prerequisites)
            .Include(c => c.AssignedStudents)
            .OrderBy(c => c.CourseNumber)
            .ToListAsync(cancellationToken);

        return courses
            .Select(c => new CourseListItem(
                c.Id,
                c.CourseNumber,
                c.Name,
                c.Description,
                c.Instructor,
                c.Department,
                c.Credits,
                c.Capacity,
                c.Capacity - c.AssignedStudents.Count,
                c.Prerequisites
                    .Select(p => p.CourseNumber)
                    .OrderBy(n => n)
                    .ToList()))
            .ToList();
    }
}

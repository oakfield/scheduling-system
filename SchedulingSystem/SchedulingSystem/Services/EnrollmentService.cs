using Microsoft.EntityFrameworkCore;
using SchedulingSystem.Data;
using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

public class EnrollmentService(IDbContextFactory<SchedulingDbContext> contextFactory) : IEnrollmentService
{
    public async Task<AssignmentResult> AssignStudentToCourseAsync(
        int studentId,
        int courseId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var student = await db.Students
            .Include(s => s.CompletedCourses)
            .Include(s => s.AssignedCourses)
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);

        var course = await db.Courses
            .Include(c => c.Prerequisites)
            .Include(c => c.AssignedStudents)
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (student is null || course is null)
        {
            return AssignmentResult.Failed("Student or course could not be found.");
        }

        var eligibility = EnrollmentValidator.Evaluate(
            EntityMappings.ToAssignmentProfile(student),
            EntityMappings.ToListItem(course));

        if (!eligibility.IsEligible)
        {
            return AssignmentResult.Failed(eligibility.Reasons);
        }

        student.AssignedCourses.Add(course);
        await db.SaveChangesAsync(cancellationToken);

        return AssignmentResult.Ok;
    }
}

using Microsoft.EntityFrameworkCore;
using SchedulingSystem.Data.Entities;

namespace SchedulingSystem.Data;

public class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.Email).IsUnique();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(c => c.CourseNumber).IsUnique();

            // Self-referencing many-to-many: which courses are prerequisites of which.
            entity.HasMany(c => c.Prerequisites)
                .WithMany(c => c.UnlocksCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursePrerequisite",
                    right => right.HasOne<Course>().WithMany().HasForeignKey("RequiredCourseId"),
                    left => left.HasOne<Course>().WithMany().HasForeignKey("CourseId"),
                    join => join.ToTable("CoursePrerequisite"));
        });

        modelBuilder.Entity<Student>()
            .HasMany(s => s.CompletedCourses)
            .WithMany(c => c.CompletedByStudents)
            .UsingEntity(join => join.ToTable("StudentCompletedCourse"));

        modelBuilder.Entity<Student>()
            .HasMany(s => s.AssignedCourses)
            .WithMany(c => c.AssignedStudents)
            .UsingEntity(join => join.ToTable("StudentAssignedCourse"));
    }
}

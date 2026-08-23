namespace SchedulingSystem.Data.Entities;

/// <summary>
/// A course offered in the upcoming semester.
/// </summary>
public class Course
{
    public int Id { get; set; }

    /// <summary>Natural key, e.g. "POTN-201". Unique.</summary>
    public required string CourseNumber { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string Instructor { get; set; }

    public int Credits { get; set; }

    public required string Department { get; set; }

    /// <summary>Total seats available in the section. Not present in source data;
    /// synthesized during seeding.</summary>
    public int Capacity { get; set; }

    /// <summary>Courses that must be completed (or assigned this semester) before this
    /// course may be taken.</summary>
    public ICollection<Course> Prerequisites { get; set; } = new List<Course>();

    /// <summary>Inverse of <see cref="Prerequisites"/>: courses that list this course as
    /// a prerequisite.</summary>
    public ICollection<Course> UnlocksCourses { get; set; } = new List<Course>();

    public ICollection<Student> CompletedByStudents { get; set; } = new List<Student>();

    public ICollection<Student> AssignedStudents { get; set; } = new List<Student>();
}

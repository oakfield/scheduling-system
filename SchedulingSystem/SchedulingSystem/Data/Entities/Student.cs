namespace SchedulingSystem.Data.Entities;

/// <summary>
/// A student who may be assigned to courses for the upcoming semester.
/// </summary>
public class Student
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    /// <summary>House / college affiliation. Purely informational.</summary>
    public required string HouseAffiliation { get; set; }

    public int YearLevel { get; set; }

    /// <summary>
    /// Credits the student is carrying in their current, already-in-progress term.
    /// Display-only: this is separate from the upcoming semester being scheduled by
    /// this app and does not count toward <see cref="MaxCreditsAllowed"/>.
    /// </summary>
    public int CurrentCredits { get; set; }

    /// <summary>Maximum credits this student may carry in the upcoming semester.</summary>
    public int MaxCreditsAllowed { get; set; }

    /// <summary>
    /// Courses this student has already completed (satisfies prerequisites, does not
    /// count toward the upcoming semester's credit load).
    /// </summary>
    public ICollection<Course> CompletedCourses { get; set; } = new List<Course>();

    /// <summary>
    /// Courses this student has been assigned to for the upcoming semester. These also
    /// satisfy prerequisites for other courses assigned in the same semester.
    /// </summary>
    public ICollection<Course> AssignedCourses { get; set; } = new List<Course>();

    public string FullName => $"{FirstName} {LastName}";
}

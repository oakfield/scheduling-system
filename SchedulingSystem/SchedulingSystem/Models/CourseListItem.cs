namespace SchedulingSystem.Models;

/// <summary>
/// Read-only projection of a course for the Courses list view. Kept separate from the EF
/// entity so the UI never touches tracked entities or triggers lazy loading.
/// </summary>
public record CourseListItem(
    int Id,
    string CourseNumber,
    string Name,
    string Description,
    string Instructor,
    string Department,
    int Credits,
    int Capacity,
    int AvailableSeats,
    IReadOnlyList<string> PrerequisiteCourseNumbers);

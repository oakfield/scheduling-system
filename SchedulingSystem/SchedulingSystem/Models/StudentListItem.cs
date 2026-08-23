namespace SchedulingSystem.Models;

/// <summary>
/// Read-only projection of a student for the Students list view. Kept separate from the
/// EF entity so the UI never touches tracked entities or triggers lazy loading.
/// </summary>
public record StudentListItem(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string HouseAffiliation,
    int YearLevel,
    int CurrentCredits,
    int MaxCreditsAllowed,
    IReadOnlyList<string> CompletedCourseNumbers)
{
    public string FullName => $"{FirstName} {LastName}";
}

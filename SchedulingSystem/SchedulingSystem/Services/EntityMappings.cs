using SchedulingSystem.Data.Entities;
using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

/// <summary>
/// Maps EF entities to read-only DTOs. Centralized so the mapping (and its behavior around
/// upcoming-term assignments and computed fields) stays identical everywhere it's used -
/// currently the Students/Courses list views and the enrollment/eligibility flow.
/// Callers must have already loaded the navigations these read (CompletedCourses,
/// AssignedCourses/AssignedStudents, Prerequisites).
/// </summary>
internal static class EntityMappings
{
    public static StudentListItem ToListItem(Student s) => new(
        s.Id,
        s.FirstName,
        s.LastName,
        s.Email,
        s.HouseAffiliation,
        s.YearLevel,
        s.CurrentCredits,
        s.MaxCreditsAllowed,
        s.CompletedCourses.Select(c => c.CourseNumber).OrderBy(c => c).ToList());

    public static StudentAssignmentProfile ToAssignmentProfile(Student s) => new(
        s.Id,
        s.FullName,
        s.Email,
        s.YearLevel,
        s.CurrentCredits,
        s.MaxCreditsAllowed,
        s.CompletedCourses.Select(c => c.CourseNumber).ToList(),
        s.AssignedCourses.Select(c => c.CourseNumber).ToList(),
        s.AssignedCourses.Sum(c => c.Credits));

    public static CourseListItem ToListItem(Course c) => new(
        c.Id,
        c.CourseNumber,
        c.Name,
        c.Description,
        c.Instructor,
        c.Department,
        c.Credits,
        c.Capacity,
        c.Capacity - c.AssignedStudents.Count,
        c.Prerequisites.Select(p => p.CourseNumber).OrderBy(n => n).ToList());
}

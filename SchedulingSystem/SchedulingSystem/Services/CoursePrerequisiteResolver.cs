using SchedulingSystem.Models;

namespace SchedulingSystem.Services;

/// <summary>
/// Resolves the transitive prerequisite chain for a target course — e.g. given a target
/// course whose direct prerequisite itself has a prerequisite, returns both. Used by the
/// Student Assignment page to help a registrar plan out of a course a student can't
/// register for yet: "what does this student need before they can take MATH-123?"
/// </summary>
public static class CoursePrerequisiteResolver
{
    /// <summary>
    /// Returns every course (direct or transitive) that must be completed before
    /// <paramref name="targetCourseNumber"/>, not including the target itself. Safe
    /// against prerequisite cycles.
    /// </summary>
    public static HashSet<string> GetTransitivePrerequisites(
        IReadOnlyList<CourseListItem> courses,
        string targetCourseNumber)
    {
        var byNumber = courses.ToDictionary(c => c.CourseNumber, StringComparer.OrdinalIgnoreCase);
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var toVisit = new Queue<string>();
        toVisit.Enqueue(targetCourseNumber);

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            if (!byNumber.TryGetValue(current, out var course))
            {
                continue;
            }

            foreach (var prerequisite in course.PrerequisiteCourseNumbers)
            {
                if (result.Add(prerequisite))
                {
                    toVisit.Enqueue(prerequisite);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Union of <see cref="GetTransitivePrerequisites(IReadOnlyList{CourseListItem}, string)"/>
    /// across several target courses at once - e.g. every prerequisite needed by any course
    /// that matched a search term.
    /// </summary>
    public static HashSet<string> GetTransitivePrerequisites(
        IReadOnlyList<CourseListItem> courses,
        IEnumerable<string> targetCourseNumbers)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var targetCourseNumber in targetCourseNumbers)
        {
            result.UnionWith(GetTransitivePrerequisites(courses, targetCourseNumber));
        }

        return result;
    }
}

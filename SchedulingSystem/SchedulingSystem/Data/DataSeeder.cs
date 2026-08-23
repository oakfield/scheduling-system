using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using SchedulingSystem.Data.Entities;

namespace SchedulingSystem.Data;

/// <summary>
/// Loads the sample students/courses CSVs into an empty database. Runs once at startup;
/// a no-op if the database already has data.
/// </summary>
public static class DataSeeder
{
    private const string StudentsFileName = "students-starter.csv";
    private const string CoursesFileName = "courses-starter.csv";

    // The source CSVs don't include seat capacity, so it's synthesized here. A fixed seed
    // keeps a from-scratch database reproducible between runs/environments.
    private static readonly Random CapacityRandom = new(42);
    private const int MinCapacity = 10;
    private const int MaxCapacity = 30; // inclusive

    public static async Task SeedAsync(
        SchedulingDbContext db,
        string? seedDataDirectory = null,
        CancellationToken cancellationToken = default)
    {
        if (await db.Students.AnyAsync(cancellationToken) || await db.Courses.AnyAsync(cancellationToken))
        {
            return;
        }

        seedDataDirectory ??= Path.Combine(AppContext.BaseDirectory, "Data", "SeedData");

        var courseRows = ReadCsv<CourseCsvRow>(Path.Combine(seedDataDirectory, CoursesFileName));
        var studentRows = ReadCsv<StudentCsvRow>(Path.Combine(seedDataDirectory, StudentsFileName));

        var coursesByNumber = new Dictionary<string, Course>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in courseRows)
        {
            var courseNumber = row.CourseNumber.Trim();
            coursesByNumber[courseNumber] = new Course
            {
                CourseNumber = courseNumber,
                Name = row.CourseName.Trim(),
                Description = row.CourseDescription.Trim(),
                Instructor = row.Instructor.Trim(),
                Credits = row.Credits,
                Department = row.Department.Trim(),
                Capacity = CapacityRandom.Next(MinCapacity, MaxCapacity + 1),
            };
        }

        db.Courses.AddRange(coursesByNumber.Values);

        // Second pass: every course now exists, so prerequisite references can be wired up.
        foreach (var row in courseRows)
        {
            var course = coursesByNumber[row.CourseNumber.Trim()];
            foreach (var prerequisiteNumber in SplitList(row.Prerequisites))
            {
                if (coursesByNumber.TryGetValue(prerequisiteNumber, out var prerequisite))
                {
                    course.Prerequisites.Add(prerequisite);
                }
            }
        }

        foreach (var row in studentRows)
        {
            var student = new Student
            {
                FirstName = row.FirstName.Trim(),
                LastName = row.LastName.Trim(),
                Email = row.Email.Trim(),
                HouseAffiliation = row.HouseAffiliation.Trim(),
                YearLevel = row.YearLevel,
                CurrentCredits = row.CurrentCredits,
                MaxCreditsAllowed = row.MaxCreditsAllowed,
            };

            foreach (var courseNumber in SplitList(row.CompletedCourses))
            {
                if (coursesByNumber.TryGetValue(courseNumber, out var course))
                {
                    student.CompletedCourses.Add(course);
                }
            }

            db.Students.Add(student);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string[] SplitList(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static List<T> ReadCsv<T>(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return [.. csv.GetRecords<T>()];
    }

    private sealed class StudentCsvRow
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string HouseAffiliation { get; set; } = "";
        public int YearLevel { get; set; }
        public int CurrentCredits { get; set; }
        public int MaxCreditsAllowed { get; set; }
        public string? CompletedCourses { get; set; }
    }

    private sealed class CourseCsvRow
    {
        public string CourseNumber { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string CourseDescription { get; set; } = "";
        public string Instructor { get; set; } = "";
        public int Credits { get; set; }
        public string? Prerequisites { get; set; }
        public string Department { get; set; } = "";
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulingSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseNumber = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Instructor = table.Column<string>(type: "text", nullable: false),
                    Credits = table.Column<int>(type: "integer", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    HouseAffiliation = table.Column<string>(type: "text", nullable: false),
                    YearLevel = table.Column<int>(type: "integer", nullable: false),
                    CurrentCredits = table.Column<int>(type: "integer", nullable: false),
                    MaxCreditsAllowed = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoursePrerequisite",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "integer", nullable: false),
                    RequiredCourseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePrerequisite", x => new { x.CourseId, x.RequiredCourseId });
                    table.ForeignKey(
                        name: "FK_CoursePrerequisite_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursePrerequisite_Courses_RequiredCourseId",
                        column: x => x.RequiredCourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssignedCourse",
                columns: table => new
                {
                    AssignedCoursesId = table.Column<int>(type: "integer", nullable: false),
                    AssignedStudentsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignedCourse", x => new { x.AssignedCoursesId, x.AssignedStudentsId });
                    table.ForeignKey(
                        name: "FK_StudentAssignedCourse_Courses_AssignedCoursesId",
                        column: x => x.AssignedCoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssignedCourse_Students_AssignedStudentsId",
                        column: x => x.AssignedStudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentCompletedCourse",
                columns: table => new
                {
                    CompletedByStudentsId = table.Column<int>(type: "integer", nullable: false),
                    CompletedCoursesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCompletedCourse", x => new { x.CompletedByStudentsId, x.CompletedCoursesId });
                    table.ForeignKey(
                        name: "FK_StudentCompletedCourse_Courses_CompletedCoursesId",
                        column: x => x.CompletedCoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCompletedCourse_Students_CompletedByStudentsId",
                        column: x => x.CompletedByStudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePrerequisite_RequiredCourseId",
                table: "CoursePrerequisite",
                column: "RequiredCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseNumber",
                table: "Courses",
                column: "CourseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignedCourse_AssignedStudentsId",
                table: "StudentAssignedCourse",
                column: "AssignedStudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCompletedCourse_CompletedCoursesId",
                table: "StudentCompletedCourse",
                column: "CompletedCoursesId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePrerequisite");

            migrationBuilder.DropTable(
                name: "StudentAssignedCourse");

            migrationBuilder.DropTable(
                name: "StudentCompletedCourse");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}

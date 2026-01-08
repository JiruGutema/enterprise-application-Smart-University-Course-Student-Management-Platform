using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartUniversity.Migrations.GradingDb
{
    /// <inheritdoc />
    public partial class AddCacheTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "course_cache",
                schema: "assessments",
                columns: table => new
                {
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    instructor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_cache", x => x.course_id);
                });

            migrationBuilder.CreateTable(
                name: "enrollment_cache",
                schema: "assessments",
                columns: table => new
                {
                    enrollment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrollment_cache", x => x.enrollment_id);
                });

            migrationBuilder.CreateTable(
                name: "student_cache",
                schema: "assessments",
                columns: table => new
                {
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_cache", x => x.student_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_cache_student_id_course_id",
                schema: "assessments",
                table: "enrollment_cache",
                columns: new[] { "student_id", "course_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_cache",
                schema: "assessments");

            migrationBuilder.DropTable(
                name: "enrollment_cache",
                schema: "assessments");

            migrationBuilder.DropTable(
                name: "student_cache",
                schema: "assessments");
        }
    }
}

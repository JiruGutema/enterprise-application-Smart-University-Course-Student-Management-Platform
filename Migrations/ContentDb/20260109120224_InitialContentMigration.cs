using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartUniversity.Migrations.ContentDb
{
    /// <inheritdoc />
    public partial class InitialContentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "materials",
                schema: "content",
                columns: table => new
                {
                    material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    file_type = table.Column<string>(type: "text", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    upload_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    size_in_bytes = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materials", x => x.material_id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    occurred_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "materials",
                schema: "content");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "content");
        }
    }
}

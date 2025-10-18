using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "todo");

            migrationBuilder.CreateTable(
                name: "content",
                schema: "todo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<byte[]>(type: "bytea", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "todo",
                schema: "todo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    description = table.Column<string>(type: "character varying(65792)", maxLength: 65792, nullable: true),
                    from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_todo", x => x.id);
                    table.CheckConstraint("CK_TimeSpan_FromBeforeTo", "\"from\" < \"to\"");
                });

            migrationBuilder.CreateTable(
                name: "attachment",
                schema: "todo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    todo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attachment", x => x.id);
                    table.ForeignKey(
                        name: "fk_attachment_content_content_id",
                        column: x => x.content_id,
                        principalSchema: "todo",
                        principalTable: "content",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attachment_todo_todo_id",
                        column: x => x.todo_id,
                        principalSchema: "todo",
                        principalTable: "todo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attachment_content_id",
                schema: "todo",
                table: "attachment",
                column: "content_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachment_todo_id",
                schema: "todo",
                table: "attachment",
                column: "todo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachment",
                schema: "todo");

            migrationBuilder.DropTable(
                name: "content",
                schema: "todo");

            migrationBuilder.DropTable(
                name: "todo",
                schema: "todo");
        }
    }
}

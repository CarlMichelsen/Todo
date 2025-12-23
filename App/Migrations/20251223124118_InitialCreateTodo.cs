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
                name: "user",
                schema: "todo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    profile_image_small = table.Column<string>(type: "text", nullable: false),
                    profile_image_medium = table.Column<string>(type: "text", nullable: true),
                    profile_image_large = table.Column<string>(type: "text", nullable: true),
                    selected_calendar_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "calendar",
                schema: "todo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    user_selected_id = table.Column<Guid>(type: "uuid", nullable: true),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_selected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar", x => x.id);
                    table.ForeignKey(
                        name: "fk_calendar_user_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "todo",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_calendar_user_user_selected_id",
                        column: x => x.user_selected_id,
                        principalSchema: "todo",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "calendar_link",
                schema: "todo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    calendar_link = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar_link", x => x.id);
                    table.ForeignKey(
                        name: "fk_calendar_link_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "todo",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event",
                schema: "todo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(1028)", maxLength: 1028, nullable: false),
                    description = table.Column<string>(type: "character varying(32896)", maxLength: 32896, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    calendar_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event", x => x.id);
                    table.ForeignKey(
                        name: "fk_event_calendar_calendar_id",
                        column: x => x.calendar_id,
                        principalSchema: "todo",
                        principalTable: "calendar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_event_user_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "todo",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "calendar_entity_calendar_link_entity",
                schema: "todo",
                columns: table => new
                {
                    calendar_links_id = table.Column<Guid>(type: "uuid", nullable: false),
                    calendars_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_calendar_entity_calendar_link_entity", x => new { x.calendar_links_id, x.calendars_id });
                    table.ForeignKey(
                        name: "fk_calendar_entity_calendar_link_entity_calendar_calendars_id",
                        column: x => x.calendars_id,
                        principalSchema: "todo",
                        principalTable: "calendar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_calendar_entity_calendar_link_entity_calendar_link_calendar",
                        column: x => x.calendar_links_id,
                        principalSchema: "todo",
                        principalTable: "calendar_link",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_calendar_owner_id",
                schema: "todo",
                table: "calendar",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_calendar_owner_id_id",
                schema: "todo",
                table: "calendar",
                columns: [ "owner_id", "id" ]);

            migrationBuilder.CreateIndex(
                name: "ix_calendar_user_selected_id",
                schema: "todo",
                table: "calendar",
                column: "user_selected_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_calendar_entity_calendar_link_entity_calendars_id",
                schema: "todo",
                table: "calendar_entity_calendar_link_entity",
                column: "calendars_id");

            migrationBuilder.CreateIndex(
                name: "ix_calendar_link_user_id",
                schema: "todo",
                table: "calendar_link",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_event_calendar_id_id",
                schema: "todo",
                table: "event",
                columns: [ "calendar_id", "id" ]);

            migrationBuilder.CreateIndex(
                name: "ix_event_calendar_id_starts_at_ends_at",
                schema: "todo",
                table: "event",
                columns: [ "calendar_id", "starts_at", "ends_at" ]);

            migrationBuilder.CreateIndex(
                name: "ix_event_created_by_id",
                schema: "todo",
                table: "event",
                column: "created_by_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calendar_entity_calendar_link_entity",
                schema: "todo");

            migrationBuilder.DropTable(
                name: "event",
                schema: "todo");

            migrationBuilder.DropTable(
                name: "calendar_link",
                schema: "todo");

            migrationBuilder.DropTable(
                name: "calendar",
                schema: "todo");

            migrationBuilder.DropTable(
                name: "user",
                schema: "todo");
        }
    }
}

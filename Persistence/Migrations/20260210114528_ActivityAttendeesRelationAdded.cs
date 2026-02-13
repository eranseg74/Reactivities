using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations;

/// <inheritdoc />
public partial class ActivityAttendeesRelationAdded : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ActivityAttendees",
            columns: table => new
            {
                UserId = table.Column<string>(type: "TEXT", nullable: false),
                ActivityId = table.Column<string>(type: "TEXT", nullable: false),
                IsHost = table.Column<bool>(type: "INTEGER", nullable: false),
                DateJoined = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ActivityAttendees", x => new { x.ActivityId, x.UserId });
                table.ForeignKey(
                    name: "FK_ActivityAttendees_Activities_ActivityId",
                    column: x => x.ActivityId,
                    principalTable: "Activities",
                    principalColumn: "Id",
                    // The cascade means that if we delete an activity, all of the related items in this table will be removed too 
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ActivityAttendees_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    // The cascade means that if we delete a user, all of the related items in this table will be removed too
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ActivityAttendees_UserId",
            table: "ActivityAttendees",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ActivityAttendees");
    }
}

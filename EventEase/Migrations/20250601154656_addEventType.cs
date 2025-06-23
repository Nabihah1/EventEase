using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class addEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventTypeID",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventType",
                columns: table => new
                {
                    EventTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTypeName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventType", x => x.EventTypeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventTypeID",
                table: "Event",
                column: "EventTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_EventType_EventTypeID",
                table: "Event",
                column: "EventTypeID",
                principalTable: "EventType",
                principalColumn: "EventTypeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_EventType_EventTypeID",
                table: "Event");

            migrationBuilder.DropTable(
                name: "EventType");

            migrationBuilder.DropIndex(
                name: "IX_Event_EventTypeID",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "EventTypeID",
                table: "Event");
        }
    }
}

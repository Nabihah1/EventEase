using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventType",
                columns: new[] { "EventTypeID", "EventTypeName" },
                values: new object[,]
                {
                    { 1, "Wedding" },
                    { 2, "Baby Shower" },
                    { 3, "Birthday" },
                    { 4, "Conference" },
                    { 5, "Reunion" },
                    { 6, "Graduation" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeID",
                keyValue: 6);
        }
    }
}

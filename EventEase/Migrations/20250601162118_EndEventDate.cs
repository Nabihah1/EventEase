using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase.Migrations
{
    /// <inheritdoc />
    public partial class EndEventDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventDate",
                table: "Event",
                newName: "StartEventDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndEventDate",
                table: "Event",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndEventDate",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "StartEventDate",
                table: "Event",
                newName: "EventDate");
        }
    }
}

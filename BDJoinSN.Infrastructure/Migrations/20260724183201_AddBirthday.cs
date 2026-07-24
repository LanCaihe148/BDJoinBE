using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BDJoinSN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBirthday : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "UserProfiles");

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "UserProfiles",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "UserProfiles");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "UserProfiles",
                type: "text",
                nullable: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp_net_auth.Migrations
{
    /// <inheritdoc />
    public partial class added_refreshTokencolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenLifeTime",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "user");

            migrationBuilder.DropColumn(
                name: "RefreshTokenLifeTime",
                table: "user");
        }
    }
}

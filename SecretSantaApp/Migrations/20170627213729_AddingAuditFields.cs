using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApp.Migrations
{
    public partial class AddingAuditFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "InsertedBy",
                "Groups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                "InsertedDateTime",
                "Groups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "InsertedBy",
                "Groups");

            migrationBuilder.DropColumn(
                "InsertedDateTime",
                "Groups");
        }
    }
}
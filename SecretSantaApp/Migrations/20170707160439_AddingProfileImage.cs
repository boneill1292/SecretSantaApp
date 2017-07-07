using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApp.Migrations
{
    public partial class AddingProfileImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "CustomUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "CustomUsers");
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApp.Migrations
{
    public partial class ChangeToAccountNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountNumber",
                table: "CustomUsers",
                newName: "AccountNumberString");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountNumberString",
                table: "CustomUsers",
                newName: "AccountNumber");
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApp.Migrations
{
    public partial class ChangingColumnNamesTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAcctNo",
                table: "MemberConditions",
                newName: "UserSelectedForConditionAcctNo");

            migrationBuilder.RenameColumn(
                name: "ConditionalUserAcctNo",
                table: "MemberConditions",
                newName: "UserReceivingConditionAcctNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserSelectedForConditionAcctNo",
                table: "MemberConditions",
                newName: "UserAcctNo");

            migrationBuilder.RenameColumn(
                name: "UserReceivingConditionAcctNo",
                table: "MemberConditions",
                newName: "ConditionalUserAcctNo");
        }
    }
}

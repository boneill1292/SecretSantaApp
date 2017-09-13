using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApp.Migrations
{
    public partial class ChangingColumnNamesTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "UserAcctNo",
                "MemberConditions",
                "UserSelectedForConditionAcctNo");

            migrationBuilder.RenameColumn(
                "ConditionalUserAcctNo",
                "MemberConditions",
                "UserReceivingConditionAcctNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                "UserSelectedForConditionAcctNo",
                "MemberConditions",
                "UserAcctNo");

            migrationBuilder.RenameColumn(
                "UserReceivingConditionAcctNo",
                "MemberConditions",
                "ConditionalUserAcctNo");
        }
    }
}
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecretSantaApp.Migrations
{
    public partial class tryingtoresync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "CustomUserDetails",
                table => new
                {
                    DetailsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    FavoriteBrands = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    PantsSize = table.Column<string>(nullable: true),
                    ShirtSize = table.Column<string>(nullable: true),
                    ShoeSize = table.Column<int>(nullable: false),
                    SweatShirtSize = table.Column<string>(nullable: true),
                    TestMigration = table.Column<string>(nullable: true),
                    UserAcctNo = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_CustomUserDetails", x => x.DetailsId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "CustomUserDetails");
        }
    }
}
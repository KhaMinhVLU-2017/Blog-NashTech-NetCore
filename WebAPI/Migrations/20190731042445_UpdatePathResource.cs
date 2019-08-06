using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPI.Migrations
{
    public partial class UpdatePathResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourcePath",
                columns: table => new
                {
                    ResourcePathID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleID = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePath", x => x.ResourcePathID);
                    table.ForeignKey(
                        name: "FK_ResourcePath_Role_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ResourcePath",
                columns: new[] { "ResourcePathID", "Path", "RoleID" },
                values: new object[] { 1, "/home/create", 3 });

            migrationBuilder.InsertData(
                table: "ResourcePath",
                columns: new[] { "ResourcePathID", "Path", "RoleID" },
                values: new object[] { 2, "/home/create/", 3 });

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePath_RoleID",
                table: "ResourcePath",
                column: "RoleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourcePath");
        }
    }
}

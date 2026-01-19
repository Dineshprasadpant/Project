using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkTrack.App.Data.Migrations
{
    /// <inheritdoc />
    public partial class Change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId1",
                table: "ProjectMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamMemberId",
                table: "ManagerTeams",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_ProjectId1",
                table: "ProjectMembers",
                column: "ProjectId1");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerTeams_TeamMemberId",
                table: "ManagerTeams",
                column: "TeamMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerTeams_AspNetUsers_TeamMemberId",
                table: "ManagerTeams",
                column: "TeamMemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId1",
                table: "ProjectMembers",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerTeams_AspNetUsers_TeamMemberId",
                table: "ManagerTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId1",
                table: "ProjectMembers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMembers_ProjectId1",
                table: "ProjectMembers");

            migrationBuilder.DropIndex(
                name: "IX_ManagerTeams_TeamMemberId",
                table: "ManagerTeams");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "TeamMemberId",
                table: "ManagerTeams");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Migrations
{
    public partial class addlinkUsertoOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "tblOrder",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_tblOrder_UserId",
                table: "tblOrder",
                column: "UserId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_tblOrder_Users_UserId",
                table: "tblOrder",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblOrder_Users_UserId",
                table: "tblOrder");

            migrationBuilder.DropIndex(
                name: "IX_tblOrder_UserId",
                table: "tblOrder");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tblOrder");
        }
    }
}

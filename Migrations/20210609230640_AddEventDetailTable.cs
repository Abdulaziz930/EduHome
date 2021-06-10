using Microsoft.EntityFrameworkCore.Migrations;

namespace EduHome.Migrations
{
    public partial class AddEventDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventDetail_Events_EventId",
                table: "EventDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventDetail",
                table: "EventDetail");

            migrationBuilder.RenameTable(
                name: "EventDetail",
                newName: "EventDetails");

            migrationBuilder.RenameIndex(
                name: "IX_EventDetail_EventId",
                table: "EventDetails",
                newName: "IX_EventDetails_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventDetails",
                table: "EventDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventDetails_Events_EventId",
                table: "EventDetails",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventDetails_Events_EventId",
                table: "EventDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventDetails",
                table: "EventDetails");

            migrationBuilder.RenameTable(
                name: "EventDetails",
                newName: "EventDetail");

            migrationBuilder.RenameIndex(
                name: "IX_EventDetails_EventId",
                table: "EventDetail",
                newName: "IX_EventDetail_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventDetail",
                table: "EventDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventDetail_Events_EventId",
                table: "EventDetail",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

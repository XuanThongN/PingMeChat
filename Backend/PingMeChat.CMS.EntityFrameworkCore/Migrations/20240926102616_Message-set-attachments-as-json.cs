using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class Messagesetattachmentsasjson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Messages_MessageId",
                table: "Attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.RenameTable(
                name: "Attachments",
                newName: "Attachment");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_MessageId",
                table: "Attachment",
                newName: "IX_Attachment_MessageId");

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6385), new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6397) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6442), new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6442) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6482), new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6483) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6486), new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6487) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6437), new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6437) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6490), new DateTime(2024, 9, 26, 17, 26, 16, 606, DateTimeKind.Local).AddTicks(6491) });

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Messages_MessageId",
                table: "Attachment",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Messages_MessageId",
                table: "Attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "Attachment",
                newName: "Attachments");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_MessageId",
                table: "Attachments",
                newName: "IX_Attachments_MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5179), new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5191) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5273), new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5274) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5282), new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5283) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5313), new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5314) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5264), new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5265) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5322), new DateTime(2024, 9, 23, 1, 12, 26, 505, DateTimeKind.Local).AddTicks(5323) });

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Messages_MessageId",
                table: "Attachments",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

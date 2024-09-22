using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class renamecontactIdintocontact_user_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Contacts");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactId",
                table: "Contacts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7024), new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7037) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7188), new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7189) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7239), new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7240) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7244), new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7245) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7182), new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7182) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7249), new DateTime(2024, 9, 23, 0, 53, 7, 262, DateTimeKind.Local).AddTicks(7249) });
        }
    }
}

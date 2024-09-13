using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class nullablecontentmessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6813), new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6824) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6911), new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6912) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6917), new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6917) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6922), new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6923) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6905), new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6906) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6937), new DateTime(2024, 9, 14, 1, 50, 6, 608, DateTimeKind.Local).AddTicks(6937) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7241), new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7253) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7317), new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7318) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7322), new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7323) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7340), new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7340) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7312), new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7313) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7345), new DateTime(2024, 9, 14, 1, 15, 23, 913, DateTimeKind.Local).AddTicks(7345) });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class Contactentitysetnicknamenullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "Contacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nickname",
                table: "Contacts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6396), new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6412) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6868), new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6869) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6878), new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6879) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6887), new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6888) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6859), new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6860) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6897), new DateTime(2024, 9, 15, 15, 54, 38, 116, DateTimeKind.Local).AddTicks(6898) });
        }
    }
}

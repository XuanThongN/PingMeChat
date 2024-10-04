using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class AccountaddReset_password_codeandResetPassWordCodeExpiryTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordCode",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordCodeExpiryTime",
                table: "Accounts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3640), new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3664) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3754), new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3754) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3793), new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3794) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3813), new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3814) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3747), new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3747) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3817), new DateTime(2024, 10, 4, 17, 20, 49, 39, DateTimeKind.Local).AddTicks(3817) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordCode",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ResetPasswordCodeExpiryTime",
                table: "Accounts");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4335), new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4350) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4418), new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4419) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4422), new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4423) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4426), new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4427) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4414), new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4414) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4431), new DateTime(2024, 10, 4, 15, 24, 0, 572, DateTimeKind.Local).AddTicks(4432) });
        }
    }
}

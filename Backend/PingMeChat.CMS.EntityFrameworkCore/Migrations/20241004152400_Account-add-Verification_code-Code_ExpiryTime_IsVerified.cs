using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class AccountaddVerification_codeCode_ExpiryTime_IsVerified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeExpiryTime",
                table: "Accounts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationCode",
                table: "Accounts",
                type: "text",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeExpiryTime",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "VerificationCode",
                table: "Accounts");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6733), new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6746) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6793), new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6794) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6797), new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6797) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6856), new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6857) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6788), new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6789) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6860), new DateTime(2024, 10, 3, 16, 41, 24, 777, DateTimeKind.Local).AddTicks(6861) });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class contactaddstatusfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Contacts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(529), new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(543) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(710), new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(712) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(718), new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(719) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(726), new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(726) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(702), new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(703) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(733), new DateTime(2024, 9, 28, 11, 9, 42, 227, DateTimeKind.Local).AddTicks(734) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contacts");

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
        }
    }
}

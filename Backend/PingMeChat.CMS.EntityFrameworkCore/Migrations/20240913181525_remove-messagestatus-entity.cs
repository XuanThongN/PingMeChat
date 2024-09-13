using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PingMeChat.CMS.EntityFrameworkCore.Migrations
{
    public partial class removemessagestatusentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageStatus");

            migrationBuilder.AddColumn<string>(
                name: "MessageReaders",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageReaders",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "MessageStatus",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    MessageId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageStatus_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageStatus_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "31feb02e-9c05-4930-a914-0af953707dfd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(208), new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(221) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "d4e5f6g7-h8i9-4j0k-1l2m-3n4o5p6q7r8s",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(322), new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(323) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "e5f6g7h8-i9j0-4k1l-2m3n-4o5p6q7r8s9t",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(333), new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(334) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "f6g7h8i9-j0k1-4l2m-3n4o-5p6q7r8s9t0u",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(344), new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(348) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "fa6f5f76-2266-4f57-8962-258fc43619dd",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(310), new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(311) });

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: "g7h8i9j0-k1l2-4m3n-4o5p-6q7r8s9t0u1v",
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(362), new DateTime(2024, 9, 12, 17, 13, 20, 586, DateTimeKind.Local).AddTicks(364) });

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatus_MessageId",
                table: "MessageStatus",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageStatus_UserId",
                table: "MessageStatus",
                column: "UserId");
        }
    }
}

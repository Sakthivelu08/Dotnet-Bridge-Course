using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BridgeCourse.Week4.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrolledOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EnrolledOn",
                table: "Students",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "student",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 109, 112, 246, 5, 103, 7, 158, 39, 164, 120, 181, 92, 61, 85, 60, 80, 177, 89, 139, 18, 208, 220, 131, 152, 235, 71, 160, 122, 205, 186, 230, 230 }, new byte[] { 93, 113, 213, 168, 51, 93, 161, 137, 179, 37, 77, 18, 201, 47, 14, 126 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "teacher",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 25, 195, 108, 163, 208, 56, 18, 142, 131, 126, 242, 77, 213, 222, 44, 164, 109, 5, 152, 170, 241, 36, 243, 94, 149, 30, 16, 237, 164, 215, 211, 201 }, new byte[] { 141, 156, 32, 134, 129, 178, 104, 167, 146, 159, 139, 93, 220, 103, 74, 37 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrolledOn",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "student",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 8, 228, 104, 70, 224, 134, 234, 189, 168, 73, 17, 128, 83, 100, 174, 111, 213, 249, 79, 39, 250, 118, 148, 255, 2, 103, 175, 175, 75, 10, 23, 14 }, new byte[] { 254, 251, 156, 93, 116, 91, 89, 96, 70, 239, 35, 45, 146, 178, 198, 69 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "teacher",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 71, 70, 44, 37, 149, 133, 122, 160, 213, 152, 83, 102, 253, 112, 163, 182, 126, 123, 120, 129, 215, 148, 242, 209, 52, 217, 26, 196, 15, 200, 219, 127 }, new byte[] { 16, 120, 65, 160, 33, 31, 139, 243, 79, 65, 137, 225, 159, 182, 215, 68 } });
        }
    }
}

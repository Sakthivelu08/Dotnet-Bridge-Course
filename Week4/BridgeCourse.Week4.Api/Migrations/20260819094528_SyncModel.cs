using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BridgeCourse.Week4.Api.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "student",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 10, 153, 143, 79, 8, 229, 42, 100, 201, 51, 118, 106, 166, 248, 81, 238, 176, 221, 232, 234, 205, 195, 159, 132, 124, 15, 152, 149, 216, 192, 195, 76 }, new byte[] { 164, 23, 42, 5, 21, 254, 65, 213, 44, 250, 79, 135, 67, 201, 122, 0 } });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "teacher",
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 151, 39, 240, 198, 110, 235, 205, 50, 75, 8, 151, 7, 117, 221, 7, 81, 32, 61, 156, 8, 222, 76, 207, 134, 218, 210, 31, 63, 166, 46, 226, 76 }, new byte[] { 11, 142, 13, 54, 238, 42, 235, 107, 1, 44, 48, 67, 97, 136, 131, 71 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApiWithRoleAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "d94d6451-d159-47e8-87ed-a0ccdc87dcb6" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d94d6451-d159-47e8-87ed-a0ccdc87dcb6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "InventoryManager", "INVENTORYMANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3", null, "Cashier", "CASHIER" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "313bb700-292c-436e-92bf-3c2c9efb8a8a", 0, "ede8461a-c47f-43f9-8a9f-9d2cc5a8b52d", "Admin@Admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAIAAYagAAAAEEZkSnsQZ6o/wF/S+KWczjoNZJHrKUGW//6wqfoWM52R9DBqz8QcBJRVbT3yowq7CA==", "13024984", true, "519edc43-79b8-4f4b-8549-091aa7f315b9", false, "admin@admin.com" },
                    { "a6cf8ea9-e09a-4a0e-bc3d-04dfb8df379d", 0, "ea2b21b3-bf07-4818-82ae-c1c85ac9e1ba", "Cashier@Cashier.com", true, false, null, "CASHIER@CASHIER.COM", "CASHIER@CASHIER.COM", "AQAAAAIAAYagAAAAEIp6WEELRf3qDbVn2EGwL124c88dyYqUf3hLZfe+SyUsKs+8IPmRYaRPB6FBGSa2pQ==", "1374987", true, "cd9feac2-165d-417c-9196-94b94606f2e1", false, "cashier@cashier.com" },
                    { "acea4f68-0798-46b9-aaf6-cb32bb2aadc0", 0, "5dbb12e9-8d22-429f-baf9-81b26559786e", "Manager@Manager.com", true, false, null, "MANAGER@MANAGER.COM", "MANAGER@MANAGER.COM", "AQAAAAIAAYagAAAAEDMyV2kX62qhV/ACA0dM81zbVHHpxFafFmafQ5huwjkoiovmG1YS5kI5qW1Hddf+eg==", "13024984", true, "31fedc61-3e9e-42da-9c84-1ff1bbac78df", false, "manager@manager.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "313bb700-292c-436e-92bf-3c2c9efb8a8a" },
                    { "3", "a6cf8ea9-e09a-4a0e-bc3d-04dfb8df379d" },
                    { "2", "acea4f68-0798-46b9-aaf6-cb32bb2aadc0" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "313bb700-292c-436e-92bf-3c2c9efb8a8a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3", "a6cf8ea9-e09a-4a0e-bc3d-04dfb8df379d" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "acea4f68-0798-46b9-aaf6-cb32bb2aadc0" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "313bb700-292c-436e-92bf-3c2c9efb8a8a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a6cf8ea9-e09a-4a0e-bc3d-04dfb8df379d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "acea4f68-0798-46b9-aaf6-cb32bb2aadc0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d94d6451-d159-47e8-87ed-a0ccdc87dcb6", 0, "521b9b83-3d0d-4f95-8da5-4c35ae379e9c", "freetrained@freetrained.com", true, false, null, "FREETRAINED@FREETRAINED.COM", "FREETRAINED@FREETRAINED.COM", "AQAAAAIAAYagAAAAEBx03SPRMcNxzketnW+if2YLK11PJ9bgSBW8VFCxVDbWmv1UG6Te2rMxEJP9L8kLYA==", "1234567890", true, "9d019822-7890-44ca-84e4-c19ff695a456", false, "freetrained@freetrained.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "d94d6451-d159-47e8-87ed-a0ccdc87dcb6" });
        }
    }
}

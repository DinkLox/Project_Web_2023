using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_2023.Migrations
{
    public partial class AddCategoriestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "83769109-3ff5-422b-b5fe-daf3287ff871", "0f47db34-3aad-4cd9-8c7b-0086d420558a" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a4e02aa0-95c0-420d-87a8-b828a09ae881", "50ffc204-d3ef-41ea-801e-74e2b07b998d" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "83769109-3ff5-422b-b5fe-daf3287ff871");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a4e02aa0-95c0-420d-87a8-b828a09ae881");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0f47db34-3aad-4cd9-8c7b-0086d420558a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "50ffc204-d3ef-41ea-801e-74e2b07b998d");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3def7b0c-f871-44ab-8d2c-51e7d1ebf65d", "00ea233a-21ac-48ff-8685-82da12e48c3d", "Khách hàng", "KHÁCH HÀNG" },
                    { "88140411-838a-448b-80ae-b10f3c203af7", "4b4c6316-db62-49f1-a0ee-c1f87b8184be", "Quản trị viên", "QUẢN TRỊ VIÊN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "Avatar", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "IsDeleted", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0d146f64-1fd5-43df-a43b-7047e1badf1b", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/img/default-user.webp", "72c411f2-77e6-45f3-a4fd-09fcab084f1f", new DateTime(2023, 11, 30, 16, 10, 36, 25, DateTimeKind.Utc).AddTicks(2830), new DateOnly(2002, 9, 2), "customer@customer.com", true, "Phan Thị Huyền", null, false, null, "CUSTOMER@CUSTOMER.COM", "CUSTOMER", "AQAAAAEAACcQAAAAEA5iNY30UhOt5UgkiiYHVPpMZ0ZJCPri04mVe9yehIBdYIE7+itKsySGJ6vEZNSPSA==", "0123456789", false, "0abdc9bc-3f55-48fd-9af1-ab714aac05a0", false, "customer" },
                    { "93de1dfe-6f40-43f6-aff6-0a3bae7fd181", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/uploads/images/admin.webp", "2ad98c58-3887-4a23-bccc-7a333abe9082", new DateTime(2023, 11, 30, 16, 10, 36, 25, DateTimeKind.Utc).AddTicks(2810), new DateOnly(2002, 7, 2), "admin@admin.com", true, "Nguyễn Ngọc Anh Tuấn", null, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAEAACcQAAAAELzA0cQQ9aa4jGNM6iMArARm/dPNZKRHHBs2s/J3TN34CyrvImSlXtzAMn3I9prTGg==", "0123456789", false, "3acb8c67-77fe-4336-9311-74b946595398", false, "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "3def7b0c-f871-44ab-8d2c-51e7d1ebf65d", "0d146f64-1fd5-43df-a43b-7047e1badf1b" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "88140411-838a-448b-80ae-b10f3c203af7", "93de1dfe-6f40-43f6-aff6-0a3bae7fd181" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories",
                column: "CreatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3def7b0c-f871-44ab-8d2c-51e7d1ebf65d", "0d146f64-1fd5-43df-a43b-7047e1badf1b" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "88140411-838a-448b-80ae-b10f3c203af7", "93de1dfe-6f40-43f6-aff6-0a3bae7fd181" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3def7b0c-f871-44ab-8d2c-51e7d1ebf65d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88140411-838a-448b-80ae-b10f3c203af7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0d146f64-1fd5-43df-a43b-7047e1badf1b");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "93de1dfe-6f40-43f6-aff6-0a3bae7fd181");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "83769109-3ff5-422b-b5fe-daf3287ff871", "a24e84c7-6659-4051-a4cb-8ff28eadb612", "Khách hàng", "KHÁCH HÀNG" },
                    { "a4e02aa0-95c0-420d-87a8-b828a09ae881", "e62bfaa8-d165-4fd7-bf2c-9360073b52b5", "Quản trị viên", "QUẢN TRỊ VIÊN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "Avatar", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "Email", "EmailConfirmed", "FullName", "IsDeleted", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "0f47db34-3aad-4cd9-8c7b-0086d420558a", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/img/default-user.webp", "fcd3263e-bd99-4035-bb27-738325bcfbea", new DateTime(2023, 11, 28, 16, 3, 58, 947, DateTimeKind.Utc).AddTicks(2390), new DateOnly(2002, 9, 2), "customer@customer.com", true, "Phan Thị Huyền", null, false, null, "CUSTOMER@CUSTOMER.COM", "CUSTOMER", "AQAAAAEAACcQAAAAEA3bCJFjCgNmppPNn6J/VJz0Aau1mG7Q/EhzM6Mqp06fJHFOHarCQOj34sTt4X7+UQ==", "0123456789", false, "e89cd5eb-2cbd-458b-9a23-9d6781afdd38", false, "customer" },
                    { "50ffc204-d3ef-41ea-801e-74e2b07b998d", 0, "Phường Bến Thuỷ, TP Vinh, Nghệ An", "/uploads/images/admin.webp", "f3aed194-09af-45d0-a608-6b8e6303b969", new DateTime(2023, 11, 28, 16, 3, 58, 947, DateTimeKind.Utc).AddTicks(2380), new DateOnly(2002, 7, 2), "admin@admin.com", true, "Nguyễn Ngọc Anh Tuấn", null, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAEAACcQAAAAEOqYl2XzlX3vPmtbJFlpfBBeygPNsMnMYJnP8O2TXvN2G4Eia0WwinxnI3fVXMwOgg==", "0123456789", false, "2ea5b3b3-3961-45ae-b0ed-a358ec28dbf4", false, "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "83769109-3ff5-422b-b5fe-daf3287ff871", "0f47db34-3aad-4cd9-8c7b-0086d420558a" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a4e02aa0-95c0-420d-87a8-b828a09ae881", "50ffc204-d3ef-41ea-801e-74e2b07b998d" });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_2023.Migrations
{
    public partial class Fixroleisnull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}

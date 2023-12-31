﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEB_2023.Migrations
{
    public partial class Adddetailfieldtoproductstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Detail",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Detail",
                table: "Products");
        }
    }
}

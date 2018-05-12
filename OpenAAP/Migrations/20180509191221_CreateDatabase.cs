using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OpenAAP.Migrations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    Email = table.Column<string>(maxLength: 64, nullable: true),
                    UserName = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordAuthentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Algorithm = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DisabledAt = table.Column<DateTime>(nullable: true),
                    Hash = table.Column<byte[]>(nullable: false),
                    IdentityId = table.Column<Guid>(nullable: false),
                    Salt = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordAuthentication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordAuthentication_Identity_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordAuthentication_IdentityId",
                table: "PasswordAuthentication",
                column: "IdentityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordAuthentication");

            migrationBuilder.DropTable(
                name: "Identity");
        }
    }
}

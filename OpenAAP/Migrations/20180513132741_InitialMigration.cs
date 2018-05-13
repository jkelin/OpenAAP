using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OpenAAP.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    description = table.Column<string>(maxLength: 255, nullable: true),
                    email = table.Column<string>(maxLength: 64, nullable: true),
                    user_name = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "password_authentication",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    algorithm = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    disabled_at = table.Column<DateTime>(nullable: true),
                    hash = table.Column<byte[]>(nullable: false),
                    identity_id = table.Column<Guid>(nullable: false),
                    salt = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_authentication", x => x.id);
                    table.ForeignKey(
                        name: "fk_password_authentication_identity_identity_id",
                        column: x => x.identity_id,
                        principalTable: "identity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    expires_at = table.Column<DateTime>(nullable: false),
                    identity_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session", x => x.id);
                    table.ForeignKey(
                        name: "fk_session_identity_identity_id",
                        column: x => x.identity_id,
                        principalTable: "identity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_password_authentication_identity_id",
                table: "password_authentication",
                column: "identity_id");

            migrationBuilder.CreateIndex(
                name: "ix_session_identity_id",
                table: "session",
                column: "identity_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "password_authentication");

            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "identity");
        }
    }
}

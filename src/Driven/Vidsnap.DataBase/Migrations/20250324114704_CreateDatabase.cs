using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vidsnap.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailUsuario = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: false),
                    NomeVideo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Extensao = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    Tamanho = table.Column<int>(type: "int", nullable: false),
                    Duracao = table.Column<int>(type: "int", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime", nullable: false),
                    URLZip = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    URLImagem = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    StatusAtual = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoStatus",
                columns: table => new
                {
                    Status = table.Column<int>(type: "int", nullable: false),
                    IdVideo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoStatus", x => new { x.IdVideo, x.Status });
                    table.ForeignKey(
                        name: "FK_VideoStatus_Video_IdVideo",
                        column: x => x.IdVideo,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoStatus");

            migrationBuilder.DropTable(
                name: "Video");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BayiAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class GuvenlikVeAyarlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HataliGirisSayisi",
                table: "Kullanici",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "KilitBitisTarihi",
                table: "Kullanici",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SifreDegistirmeli",
                table: "Kullanici",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SonGirisTarihi",
                table: "Kullanici",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FirmaAyar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unvan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VergiNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    VergiDairesi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Eposta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    VarsayilanKdvOrani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    VarsayilanVadeGun = table.Column<int>(type: "int", nullable: false),
                    KritikStokUyarisi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirmaAyar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NumaraSerisi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Onek = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Yil = table.Column<int>(type: "int", nullable: false),
                    SonNumara = table.Column<int>(type: "int", nullable: false),
                    Uzunluk = table.Column<byte>(type: "tinyint", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumaraSerisi", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FirmaAyar",
                columns: new[] { "Id", "Adres", "Eposta", "KayitTarihi", "KritikStokUyarisi", "Logo", "Telefon", "Unvan", "VarsayilanKdvOrani", "VarsayilanVadeGun", "VergiDairesi", "VergiNo" },
                values: new object[] { 1, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, "BayiAI Demo Dağıtım A.Ş.", 20m, 30, null, null });

            migrationBuilder.InsertData(
                table: "NumaraSerisi",
                columns: new[] { "Id", "KayitTarihi", "Kod", "Onek", "SonNumara", "Uzunluk", "Yil" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "BAY", "B", 0, (byte)4, 2026 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SIP", "SIP", 0, (byte)6, 2026 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IRS", "IRS", 0, (byte)6, 2026 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "FTR", "FTR", 0, (byte)6, 2026 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "THS", "THS", 0, (byte)6, 2026 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NumaraSerisi_Kod",
                table: "NumaraSerisi",
                column: "Kod",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FirmaAyar");

            migrationBuilder.DropTable(
                name: "NumaraSerisi");

            migrationBuilder.DropColumn(
                name: "HataliGirisSayisi",
                table: "Kullanici");

            migrationBuilder.DropColumn(
                name: "KilitBitisTarihi",
                table: "Kullanici");

            migrationBuilder.DropColumn(
                name: "SifreDegistirmeli",
                table: "Kullanici");

            migrationBuilder.DropColumn(
                name: "SonGirisTarihi",
                table: "Kullanici");
        }
    }
}

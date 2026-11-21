using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BayiAI.Data.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bolge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KasaBanka",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tip = table.Column<byte>(type: "tinyint", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KasaBanka", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UstKategoriId = table.Column<int>(type: "int", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategori", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kategori_Kategori_UstKategoriId",
                        column: x => x.UstKategoriId,
                        principalTable: "Kategori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kullanici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Rol = table.Column<byte>(type: "tinyint", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanici", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunKodu = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ListeFiyati = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvOrani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    KritikStok = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    StokMiktari = table.Column<int>(type: "int", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urun", x => x.Id);
                    table.CheckConstraint("CK_Urun_Fiyat", "[ListeFiyati] >= 0");
                    table.CheckConstraint("CK_Urun_Stok", "[StokMiktari] >= 0");
                    table.ForeignKey(
                        name: "FK_Urun_Kategori_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AiIslem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslemTipi = table.Column<byte>(type: "tinyint", nullable: false),
                    Girdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cikti = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SureMs = table.Column<int>(type: "int", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiIslem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiIslem_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bayi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BayiKodu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Unvan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VergiNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    VergiDairesi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    YetkiliAdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Eposta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    BolgeId = table.Column<int>(type: "int", nullable: false),
                    SorumluKullaniciId = table.Column<int>(type: "int", nullable: true),
                    IskontoOrani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RiskLimiti = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bayi", x => x.Id);
                    table.CheckConstraint("CK_Bayi_Iskonto", "[IskontoOrani] BETWEEN 0 AND 50");
                    table.CheckConstraint("CK_Bayi_Risk", "[RiskLimiti] >= 0");
                    table.ForeignKey(
                        name: "FK_Bayi_Bolge_BolgeId",
                        column: x => x.BolgeId,
                        principalTable: "Bolge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bayi_Kullanici_SorumluKullaniciId",
                        column: x => x.SorumluKullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IslemLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TabloAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KayitId = table.Column<int>(type: "int", nullable: false),
                    Islem = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EskiDeger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YeniDeger = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IslemLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IslemLog_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Siparis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiparisNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    SiparisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<byte>(type: "tinyint", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IskontoTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BayiId = table.Column<int>(type: "int", nullable: false),
                    AraToplam = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GenelToplam = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Siparis_Bayi_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Siparis_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tahsilat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BayiId = table.Column<int>(type: "int", nullable: false),
                    KasaBankaId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdemeTipi = table.Column<byte>(type: "tinyint", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tahsilat", x => x.Id);
                    table.CheckConstraint("CK_Tahsilat_Tutar", "[Tutar] > 0");
                    table.ForeignKey(
                        name: "FK_Tahsilat_Bayi_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tahsilat_KasaBanka_KasaBankaId",
                        column: x => x.KasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tahsilat_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fatura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaturaNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SiparisId = table.Column<int>(type: "int", nullable: false),
                    FaturaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VadeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BayiId = table.Column<int>(type: "int", nullable: false),
                    AraToplam = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KdvTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GenelToplam = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fatura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fatura_Bayi_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fatura_Siparis_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sevkiyat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiparisId = table.Column<int>(type: "int", nullable: false),
                    SevkTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AracPlaka = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    KargoFirmasi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TakipNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TeslimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sevkiyat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sevkiyat_Siparis_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiparisKalemi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiparisId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<int>(type: "int", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IskontoOrani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    KdvOrani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SatirToplam = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisKalemi", x => x.Id);
                    table.CheckConstraint("CK_SiparisKalemi_Miktar", "[Miktar] > 0");
                    table.ForeignKey(
                        name: "FK_SiparisKalemi_Siparis_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiparisKalemi_Urun_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CariHareket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BayiId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IslemTipi = table.Column<byte>(type: "tinyint", nullable: false),
                    FaturaId = table.Column<int>(type: "int", nullable: true),
                    TahsilatId = table.Column<int>(type: "int", nullable: true),
                    Borc = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Alacak = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CariHareket", x => x.Id);
                    table.CheckConstraint("CK_CariHareket_Tutar", "[Borc] >= 0 AND [Alacak] >= 0 AND ([Borc] > 0 OR [Alacak] > 0)");
                    table.ForeignKey(
                        name: "FK_CariHareket_Bayi_BayiId",
                        column: x => x.BayiId,
                        principalTable: "Bayi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CariHareket_Fatura_FaturaId",
                        column: x => x.FaturaId,
                        principalTable: "Fatura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CariHareket_Tahsilat_TahsilatId",
                        column: x => x.TahsilatId,
                        principalTable: "Tahsilat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StokHareket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HareketTipi = table.Column<byte>(type: "tinyint", nullable: false),
                    Miktar = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SevkiyatId = table.Column<int>(type: "int", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    SilindiMi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StokHareket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StokHareket_Kullanici_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StokHareket_Sevkiyat_SevkiyatId",
                        column: x => x.SevkiyatId,
                        principalTable: "Sevkiyat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StokHareket_Urun_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiIslem_KullaniciId",
                table: "AiIslem",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Bayi_BayiKodu",
                table: "Bayi",
                column: "BayiKodu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bayi_BolgeId",
                table: "Bayi",
                column: "BolgeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bayi_SorumluKullaniciId",
                table: "Bayi",
                column: "SorumluKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Bayi_VergiNo",
                table: "Bayi",
                column: "VergiNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bolge_Ad",
                table: "Bolge",
                column: "Ad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CariHareket_BayiId",
                table: "CariHareket",
                column: "BayiId")
                .Annotation("SqlServer:Include", new[] { "Borc", "Alacak" });

            migrationBuilder.CreateIndex(
                name: "IX_CariHareket_FaturaId",
                table: "CariHareket",
                column: "FaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_CariHareket_TahsilatId",
                table: "CariHareket",
                column: "TahsilatId");

            migrationBuilder.CreateIndex(
                name: "IX_Fatura_BayiId",
                table: "Fatura",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Fatura_FaturaNo",
                table: "Fatura",
                column: "FaturaNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fatura_SiparisId",
                table: "Fatura",
                column: "SiparisId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fatura_VadeTarihi",
                table: "Fatura",
                column: "VadeTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_IslemLog_KullaniciId",
                table: "IslemLog",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaBanka_Ad",
                table: "KasaBanka",
                column: "Ad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kategori_UstKategoriId",
                table: "Kategori",
                column: "UstKategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanici_KullaniciAdi",
                table: "Kullanici",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sevkiyat_SiparisId",
                table: "Sevkiyat",
                column: "SiparisId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparis_BayiId",
                table: "Siparis",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparis_KullaniciId",
                table: "Siparis",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparis_SiparisNo",
                table: "Siparis",
                column: "SiparisNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Siparis_SiparisTarihi",
                table: "Siparis",
                column: "SiparisTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisKalemi_SiparisId_UrunId",
                table: "SiparisKalemi",
                columns: new[] { "SiparisId", "UrunId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiparisKalemi_UrunId",
                table: "SiparisKalemi",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareket_KullaniciId",
                table: "StokHareket",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareket_SevkiyatId",
                table: "StokHareket",
                column: "SevkiyatId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareket_UrunId",
                table: "StokHareket",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_Tahsilat_BayiId",
                table: "Tahsilat",
                column: "BayiId");

            migrationBuilder.CreateIndex(
                name: "IX_Tahsilat_KasaBankaId",
                table: "Tahsilat",
                column: "KasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tahsilat_KullaniciId",
                table: "Tahsilat",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Urun_KategoriId",
                table: "Urun",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_Urun_UrunKodu",
                table: "Urun",
                column: "UrunKodu",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiIslem");

            migrationBuilder.DropTable(
                name: "CariHareket");

            migrationBuilder.DropTable(
                name: "IslemLog");

            migrationBuilder.DropTable(
                name: "SiparisKalemi");

            migrationBuilder.DropTable(
                name: "StokHareket");

            migrationBuilder.DropTable(
                name: "Fatura");

            migrationBuilder.DropTable(
                name: "Tahsilat");

            migrationBuilder.DropTable(
                name: "Sevkiyat");

            migrationBuilder.DropTable(
                name: "Urun");

            migrationBuilder.DropTable(
                name: "KasaBanka");

            migrationBuilder.DropTable(
                name: "Siparis");

            migrationBuilder.DropTable(
                name: "Kategori");

            migrationBuilder.DropTable(
                name: "Bayi");

            migrationBuilder.DropTable(
                name: "Bolge");

            migrationBuilder.DropTable(
                name: "Kullanici");
        }
    }
}

/* =====================================================================
   BayiAI - Yapay Zeka Destekli Bayi Yönetim ve Ön Muhasebe Otomasyonu
   01_tablolar.sql : Veritabanı ve 15 tablonun oluşturulması

   Amaç: Tabloları önce düz SQL ile görüp anlamak.
   Uygulamada aynı tabloları EF Core migration oluşturacak; bu script
   öğrenme ve rapordaki "Veritabanı tasarımı" bölümü içindir.

   Çalıştırma: SQL Server Management Studio veya Visual Studio
   "SQL Server Object Explorer" -> (localdb)\MSSQLLocalDB -> New Query
   ===================================================================== */

IF DB_ID('BayiAI') IS NULL
    CREATE DATABASE BayiAI;
GO
USE BayiAI;
GO

/* ---------------------------------------------------------------------
   1) TANIMLAR
   --------------------------------------------------------------------- */

CREATE TABLE Kullanici (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    AdSoyad       NVARCHAR(100) NOT NULL,
    KullaniciAdi  NVARCHAR(50)  NOT NULL CONSTRAINT UQ_Kullanici_KullaniciAdi UNIQUE,
    SifreHash     NVARCHAR(256) NOT NULL,          -- şifrenin kendisi DEĞİL, hash'i
    Rol           TINYINT       NOT NULL,          -- 1 Admin, 2 Satis, 3 Depo, 4 Muhasebe
    AktifMi       BIT           NOT NULL DEFAULT 1,
    KayitTarihi   DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT CK_Kullanici_Rol CHECK (Rol BETWEEN 1 AND 4)
);

CREATE TABLE Bolge (
    Id  INT IDENTITY(1,1) PRIMARY KEY,
    Ad  NVARCHAR(50) NOT NULL CONSTRAINT UQ_Bolge_Ad UNIQUE
);

CREATE TABLE Kategori (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Ad              NVARCHAR(100) NOT NULL,
    UstKategoriId   INT NULL,                       -- kendine bağlanan (self-reference) ilişki
    CONSTRAINT FK_Kategori_UstKategori FOREIGN KEY (UstKategoriId) REFERENCES Kategori(Id)
);

/* ---------------------------------------------------------------------
   2) BAYİ (merkez tablo)
   --------------------------------------------------------------------- */

CREATE TABLE Bayi (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    BayiKodu            NVARCHAR(20)  NOT NULL CONSTRAINT UQ_Bayi_BayiKodu UNIQUE,
    Unvan               NVARCHAR(200) NOT NULL,
    VergiNo             NVARCHAR(11)  NOT NULL CONSTRAINT UQ_Bayi_VergiNo UNIQUE,
    VergiDairesi        NVARCHAR(100) NULL,
    YetkiliAdSoyad      NVARCHAR(100) NULL,
    Telefon             NVARCHAR(20)  NULL,
    Eposta              NVARCHAR(100) NULL,
    Adres               NVARCHAR(400) NULL,
    BolgeId             INT NOT NULL,
    SorumluKullaniciId  INT NULL,
    IskontoOrani        DECIMAL(5,2)  NOT NULL DEFAULT 0,
    RiskLimiti          DECIMAL(18,2) NOT NULL DEFAULT 0,
    AktifMi             BIT NOT NULL DEFAULT 1,
    SilindiMi           BIT NOT NULL DEFAULT 0,     -- soft delete
    KayitTarihi         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Bayi_Bolge     FOREIGN KEY (BolgeId) REFERENCES Bolge(Id),
    CONSTRAINT FK_Bayi_Kullanici FOREIGN KEY (SorumluKullaniciId) REFERENCES Kullanici(Id),
    CONSTRAINT CK_Bayi_Iskonto   CHECK (IskontoOrani BETWEEN 0 AND 50),
    CONSTRAINT CK_Bayi_Risk      CHECK (RiskLimiti >= 0)
);

/* ---------------------------------------------------------------------
   3) ÜRÜN VE STOK
   --------------------------------------------------------------------- */

CREATE TABLE Urun (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    UrunKodu     NVARCHAR(30)  NOT NULL CONSTRAINT UQ_Urun_UrunKodu UNIQUE,
    Ad           NVARCHAR(200) NOT NULL,
    KategoriId   INT NOT NULL,
    Birim        NVARCHAR(20)  NOT NULL DEFAULT N'Adet',
    ListeFiyati  DECIMAL(18,2) NOT NULL,
    KdvOrani     DECIMAL(5,2)  NOT NULL DEFAULT 20,
    StokMiktari  INT NOT NULL DEFAULT 0,
    KritikStok   INT NOT NULL DEFAULT 0,
    AktifMi      BIT NOT NULL DEFAULT 1,
    SilindiMi    BIT NOT NULL DEFAULT 0,
    KayitTarihi  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Urun_Kategori FOREIGN KEY (KategoriId) REFERENCES Kategori(Id),
    CONSTRAINT CK_Urun_Fiyat    CHECK (ListeFiyati >= 0),
    CONSTRAINT CK_Urun_Stok     CHECK (StokMiktari >= 0)   -- stok eksiye düşemez
);

/* ---------------------------------------------------------------------
   4) SATIŞ
   --------------------------------------------------------------------- */

CREATE TABLE Siparis (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    SiparisNo      NVARCHAR(20) NOT NULL CONSTRAINT UQ_Siparis_SiparisNo UNIQUE,
    BayiId         INT NOT NULL,
    KullaniciId    INT NOT NULL,
    SiparisTarihi  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Durum          TINYINT NOT NULL DEFAULT 1,  -- 1 Beklemede, 2 Onaylandi, 3 SevkEdildi, 4 TeslimEdildi, 5 Iptal
    Aciklama       NVARCHAR(500) NULL,
    AraToplam      DECIMAL(18,2) NOT NULL DEFAULT 0,
    IskontoTutari  DECIMAL(18,2) NOT NULL DEFAULT 0,
    KdvTutari      DECIMAL(18,2) NOT NULL DEFAULT 0,
    GenelToplam    DECIMAL(18,2) NOT NULL DEFAULT 0,
    SilindiMi      BIT NOT NULL DEFAULT 0,
    KayitTarihi    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Siparis_Bayi      FOREIGN KEY (BayiId) REFERENCES Bayi(Id),
    CONSTRAINT FK_Siparis_Kullanici FOREIGN KEY (KullaniciId) REFERENCES Kullanici(Id),
    CONSTRAINT CK_Siparis_Durum     CHECK (Durum BETWEEN 1 AND 5)
);

-- Sipariş ile Ürün arasındaki çoka-çok (N-N) ilişkinin ara tablosu
CREATE TABLE SiparisKalemi (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    SiparisId     INT NOT NULL,
    UrunId        INT NOT NULL,
    Miktar        INT NOT NULL,
    BirimFiyat    DECIMAL(18,2) NOT NULL,   -- sipariş anındaki fiyatın kopyası (snapshot)
    IskontoOrani  DECIMAL(5,2)  NOT NULL DEFAULT 0,
    KdvOrani      DECIMAL(5,2)  NOT NULL,
    SatirToplam   DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_SiparisKalemi_Siparis FOREIGN KEY (SiparisId) REFERENCES Siparis(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SiparisKalemi_Urun    FOREIGN KEY (UrunId) REFERENCES Urun(Id),
    CONSTRAINT UQ_SiparisKalemi         UNIQUE (SiparisId, UrunId),
    CONSTRAINT CK_SiparisKalemi_Miktar  CHECK (Miktar > 0)
);

CREATE TABLE Sevkiyat (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    SiparisId     INT NOT NULL,
    SevkTarihi    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    AracPlaka     NVARCHAR(15)  NULL,
    KargoFirmasi  NVARCHAR(100) NULL,
    TakipNo       NVARCHAR(50)  NULL,
    TeslimTarihi  DATETIME2 NULL,
    CONSTRAINT FK_Sevkiyat_Siparis FOREIGN KEY (SiparisId) REFERENCES Siparis(Id)
);

-- Stok neden değişti? Her giriş/çıkışın kaydı
CREATE TABLE StokHareket (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    UrunId        INT NOT NULL,
    Tarih         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    HareketTipi   TINYINT NOT NULL,    -- 1 Giris, 2 Cikis, 3 Sayim
    Miktar        INT NOT NULL,
    Aciklama      NVARCHAR(300) NULL,
    SevkiyatId    INT NULL,
    KullaniciId   INT NOT NULL,
    CONSTRAINT FK_StokHareket_Urun      FOREIGN KEY (UrunId) REFERENCES Urun(Id),
    CONSTRAINT FK_StokHareket_Sevkiyat  FOREIGN KEY (SevkiyatId) REFERENCES Sevkiyat(Id),
    CONSTRAINT FK_StokHareket_Kullanici FOREIGN KEY (KullaniciId) REFERENCES Kullanici(Id),
    CONSTRAINT CK_StokHareket_Tip       CHECK (HareketTipi BETWEEN 1 AND 3)
);

/* ---------------------------------------------------------------------
   5) MUHASEBE
   --------------------------------------------------------------------- */

CREATE TABLE Fatura (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    FaturaNo     NVARCHAR(20) NOT NULL CONSTRAINT UQ_Fatura_FaturaNo UNIQUE,
    SiparisId    INT NOT NULL CONSTRAINT UQ_Fatura_SiparisId UNIQUE,  -- FK + UNIQUE = bire-bir (1-1)
    BayiId       INT NOT NULL,
    FaturaTarihi DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    VadeTarihi   DATE NOT NULL,
    AraToplam    DECIMAL(18,2) NOT NULL,
    KdvTutari    DECIMAL(18,2) NOT NULL,
    GenelToplam  DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_Fatura_Siparis FOREIGN KEY (SiparisId) REFERENCES Siparis(Id),
    CONSTRAINT FK_Fatura_Bayi    FOREIGN KEY (BayiId) REFERENCES Bayi(Id)
);

CREATE TABLE KasaBanka (
    Id    INT IDENTITY(1,1) PRIMARY KEY,
    Ad    NVARCHAR(100) NOT NULL CONSTRAINT UQ_KasaBanka_Ad UNIQUE,
    Tip   TINYINT NOT NULL,             -- 1 Kasa, 2 Banka
    Iban  NVARCHAR(34) NULL,
    CONSTRAINT CK_KasaBanka_Tip CHECK (Tip IN (1, 2))
);

CREATE TABLE Tahsilat (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    BayiId       INT NOT NULL,
    KasaBankaId  INT NOT NULL,
    Tarih        DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Tutar        DECIMAL(18,2) NOT NULL,
    OdemeTipi    TINYINT NOT NULL,      -- 1 Nakit, 2 Havale, 3 Cek, 4 KrediKarti
    Aciklama     NVARCHAR(300) NULL,
    KullaniciId  INT NOT NULL,
    CONSTRAINT FK_Tahsilat_Bayi      FOREIGN KEY (BayiId) REFERENCES Bayi(Id),
    CONSTRAINT FK_Tahsilat_KasaBanka FOREIGN KEY (KasaBankaId) REFERENCES KasaBanka(Id),
    CONSTRAINT FK_Tahsilat_Kullanici FOREIGN KEY (KullaniciId) REFERENCES Kullanici(Id),
    CONSTRAINT CK_Tahsilat_Tutar     CHECK (Tutar > 0),
    CONSTRAINT CK_Tahsilat_OdemeTipi CHECK (OdemeTipi BETWEEN 1 AND 4)
);

-- Bayinin borç-alacak defteri. Bakiye = SUM(Borc) - SUM(Alacak)
CREATE TABLE CariHareket (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    BayiId      INT NOT NULL,
    Tarih       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IslemTipi   TINYINT NOT NULL,       -- 1 Fatura, 2 Tahsilat, 3 Iade, 4 Duzeltme
    FaturaId    INT NULL,
    TahsilatId  INT NULL,
    Borc        DECIMAL(18,2) NOT NULL DEFAULT 0,
    Alacak      DECIMAL(18,2) NOT NULL DEFAULT 0,
    Aciklama    NVARCHAR(300) NULL,
    CONSTRAINT FK_CariHareket_Bayi     FOREIGN KEY (BayiId) REFERENCES Bayi(Id),
    CONSTRAINT FK_CariHareket_Fatura   FOREIGN KEY (FaturaId) REFERENCES Fatura(Id),
    CONSTRAINT FK_CariHareket_Tahsilat FOREIGN KEY (TahsilatId) REFERENCES Tahsilat(Id),
    CONSTRAINT CK_CariHareket_Tip      CHECK (IslemTipi BETWEEN 1 AND 4),
    CONSTRAINT CK_CariHareket_Tutar    CHECK (Borc >= 0 AND Alacak >= 0 AND (Borc > 0 OR Alacak > 0))
);

/* ---------------------------------------------------------------------
   6) SİSTEM
   --------------------------------------------------------------------- */

CREATE TABLE IslemLog (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciId  INT NULL,
    Tarih        DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    TabloAdi     NVARCHAR(50) NOT NULL,
    KayitId      INT NOT NULL,
    Islem        NVARCHAR(10) NOT NULL,      -- Ekle / Guncelle / Sil
    EskiDeger    NVARCHAR(MAX) NULL,
    YeniDeger    NVARCHAR(MAX) NULL,
    CONSTRAINT FK_IslemLog_Kullanici FOREIGN KEY (KullaniciId) REFERENCES Kullanici(Id)
);

CREATE TABLE AiIslem (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciId  INT NULL,
    Tarih        DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IslemTipi    TINYINT NOT NULL,           -- 1 SiparisTahmini, 2 RiskYorumu, 3 DogalDilSorgu
    Girdi        NVARCHAR(MAX) NOT NULL,
    Cikti        NVARCHAR(MAX) NULL,
    SureMs       INT NULL,
    CONSTRAINT FK_AiIslem_Kullanici FOREIGN KEY (KullaniciId) REFERENCES Kullanici(Id)
);

/* ---------------------------------------------------------------------
   7) INDEX'LER (sık aranan kolonlar)
   PK ve UNIQUE kolonlarda SQL Server zaten index oluşturur.
   FK kolonlarında OTOMATİK oluşturmaz, bu yüzden elle ekliyoruz.
   --------------------------------------------------------------------- */

CREATE INDEX IX_Bayi_BolgeId          ON Bayi(BolgeId);
CREATE INDEX IX_Siparis_BayiId        ON Siparis(BayiId);
CREATE INDEX IX_Siparis_SiparisTarihi ON Siparis(SiparisTarihi);
CREATE INDEX IX_Fatura_BayiId         ON Fatura(BayiId);
CREATE INDEX IX_Fatura_VadeTarihi     ON Fatura(VadeTarihi);
CREATE INDEX IX_CariHareket_BayiId    ON CariHareket(BayiId) INCLUDE (Borc, Alacak);
CREATE INDEX IX_StokHareket_UrunId    ON StokHareket(UrunId);
GO

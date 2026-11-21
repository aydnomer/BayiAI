/* =====================================================================
   BayiAI - 02_ornek_veri.sql : Demo ve test için örnek veri
   Veritabanı oluşturulduktan SONRA çalıştırın (01_tablolar.sql veya EF Core migration ile).
   ===================================================================== */
USE BayiAI;
GO

-- Hem 01_tablolar.sql ile hem de EF Core migration ile oluşturulan veritabanında çalışır.

-- Kullanıcılar (SifreHash burada örnek; uygulama gerçek hash üretecek)
INSERT INTO Kullanici (AdSoyad, KullaniciAdi, SifreHash, Rol, AktifMi) VALUES
 (N'Sistem Yöneticisi', N'admin',    N'ORNEK_HASH', 1, 1),
 (N'Ayşe Satış',        N'ayse',     N'ORNEK_HASH', 2, 1),
 (N'Mehmet Depo',       N'mehmet',   N'ORNEK_HASH', 3, 1),
 (N'Zeynep Muhasebe',   N'zeynep',   N'ORNEK_HASH', 4, 1);

INSERT INTO Bolge (Ad) VALUES (N'Marmara'), (N'Ege'), (N'İç Anadolu'), (N'Akdeniz'), (N'Karadeniz');

-- Kategori ağacı: üst kategori + alt kategoriler (self-reference)
INSERT INTO Kategori (Ad, UstKategoriId) VALUES (N'İçecek', NULL), (N'Atıştırmalık', NULL);
INSERT INTO Kategori (Ad, UstKategoriId) VALUES
 (N'Gazlı İçecek', 1), (N'Meyve Suyu', 1), (N'Cips', 2), (N'Bisküvi', 2);

INSERT INTO Urun (UrunKodu, Ad, KategoriId, Birim, ListeFiyati, KdvOrani, StokMiktari, KritikStok, AktifMi) VALUES
 (N'GZ-001', N'Kola 1 L (12''li koli)',        3, N'Koli', 360.00, 20, 500, 50, 1),
 (N'GZ-002', N'Gazoz 1 L (12''li koli)',       3, N'Koli', 300.00, 20, 300, 40, 1),
 (N'MS-001', N'Vişne Suyu 1 L (12''li koli)',  4, N'Koli', 420.00, 20, 200, 30, 1),
 (N'CP-001', N'Patates Cipsi (24''lü koli)',   5, N'Koli', 540.00, 20, 150, 20, 1),
 (N'BS-001', N'Çikolatalı Bisküvi (36''lı)',   6, N'Koli', 480.00, 20,  25, 30, 1);  -- kritik stok altında

INSERT INTO Bayi (BayiKodu, Unvan, VergiNo, VergiDairesi, YetkiliAdSoyad, Telefon, BolgeId, SorumluKullaniciId, IskontoOrani, RiskLimiti, AktifMi) VALUES
 (N'B-0001', N'Yıldız Gıda Ltd. Şti.',      N'1234567890', N'Kadıköy',   N'Ali Yıldız',   N'0216 000 00 01', 1, 2, 10, 150000, 1),
 (N'B-0002', N'Ege Toptan Gıda A.Ş.',       N'2345678901', N'Konak',     N'Elif Kaya',    N'0232 000 00 02', 2, 2, 12, 250000, 1),
 (N'B-0003', N'Başkent Dağıtım Ltd. Şti.',  N'3456789012', N'Çankaya',   N'Murat Demir',  N'0312 000 00 03', 3, 2,  8, 100000, 1),
 (N'B-0004', N'Akdeniz Market Zinciri',     N'4567890123', N'Muratpaşa', N'Selin Ak',     N'0242 000 00 04', 4, 2, 15, 300000, 1),
 (N'B-0005', N'Karadeniz Bakkallar Birliği',N'5678901234', N'Ortahisar', N'Hasan Çelik',  N'0462 000 00 05', 5, 2,  5,  50000, 1);

INSERT INTO KasaBanka (Ad, Tip, Iban) VALUES
 (N'Merkez Kasa', 1, NULL),
 (N'Banka TL Hesabı', 2, N'TR000000000000000000000000');
GO

using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BayiAI.Data.Configurations;

// Configuration sınıfları: C# sınıfının SQL tablosuna NASIL dönüşeceğini anlatır
// (tablo adı, kolon uzunluğu, zorunlu mu, index, başlangıç verisi...). Buna "Fluent API" denir.

public class IslemLogConfiguration : IEntityTypeConfiguration<IslemLog>
{
    public void Configure(EntityTypeBuilder<IslemLog> b)
    {
        b.ToTable("IslemLog");
        b.Property(x => x.TabloAdi).HasMaxLength(50).IsRequired();
        b.Property(x => x.Islem).HasMaxLength(10).IsRequired();
        b.HasOne<Kullanici>().WithMany().HasForeignKey(x => x.KullaniciId);
    }
}

public class AiIslemConfiguration : IEntityTypeConfiguration<AiIslem>
{
    public void Configure(EntityTypeBuilder<AiIslem> b)
    {
        b.ToTable("AiIslem");
        b.Property(x => x.Girdi).IsRequired();
        b.HasOne<Kullanici>().WithMany().HasForeignKey(x => x.KullaniciId);
    }
}

// YENİ: FirmaAyar tablosunun ayarları.
public class FirmaAyarConfiguration : IEntityTypeConfiguration<FirmaAyar>
{
    public void Configure(EntityTypeBuilder<FirmaAyar> b)
    {
        // SQL'deki tablo adı.
        b.ToTable("FirmaAyar");

        // HasMaxLength(200): SQL'de nvarchar(200). IsRequired: NOT NULL (boş olamaz).
        b.Property(x => x.Unvan).HasMaxLength(200).IsRequired();
        b.Property(x => x.VergiNo).HasMaxLength(11);          // VKN 10, TCKN 11 hane
        b.Property(x => x.VergiDairesi).HasMaxLength(100);
        b.Property(x => x.Adres).HasMaxLength(400);
        b.Property(x => x.Telefon).HasMaxLength(20);
        b.Property(x => x.Eposta).HasMaxLength(100);

        // HasPrecision(5, 2): decimal(5,2) → en fazla 999.99 (oran için yeterli).
        b.Property(x => x.VarsayilanKdvOrani).HasPrecision(5, 2);

        // HasData (seed data): Migration çalışınca tabloya bu satır otomatik eklenir.
        // new { ... }: Anonim nesne. KayitTarihi gibi dışarıdan atanamayan alanlara da değer verebiliyoruz.
        b.HasData(new
        {
            Id = 1,
            Unvan = "BayiAI Demo Dağıtım A.Ş.",
            VarsayilanKdvOrani = 20m,             // m: decimal sayı demek
            VarsayilanVadeGun = 30,
            KritikStokUyarisi = true,
            KayitTarihi = new DateTime(2026, 1, 1), // Sabit tarih: her migration'da aynı kalsın
            SilindiMi = false
        });
    }
}

// YENİ: NumaraSerisi tablosunun ayarları.
public class NumaraSerisiConfiguration : IEntityTypeConfiguration<NumaraSerisi>
{
    public void Configure(EntityTypeBuilder<NumaraSerisi> b)
    {
        b.ToTable("NumaraSerisi");
        b.Property(x => x.Kod).HasMaxLength(10).IsRequired();
        b.Property(x => x.Onek).HasMaxLength(10).IsRequired();

        // UNIQUE index: Aynı koddan (örn. iki tane "FTR") iki satır olamaz.
        b.HasIndex(x => x.Kod).IsUnique();

        // 5 belge türü için başlangıç satırları.
        // (byte)4: 4 sayısını byte tipine çeviriyoruz (cast), çünkü Uzunluk byte.
        var tarih = new DateTime(2026, 1, 1);
        b.HasData(
            new { Id = 1, Kod = "BAY", Onek = "B", Yil = 2026, SonNumara = 0, Uzunluk = (byte)4, KayitTarihi = tarih, SilindiMi = false }, // Bayi kodu: B20260001
            new { Id = 2, Kod = "SIP", Onek = "SIP", Yil = 2026, SonNumara = 0, Uzunluk = (byte)6, KayitTarihi = tarih, SilindiMi = false }, // Sipariş
            new { Id = 3, Kod = "IRS", Onek = "IRS", Yil = 2026, SonNumara = 0, Uzunluk = (byte)6, KayitTarihi = tarih, SilindiMi = false }, // İrsaliye
            new { Id = 4, Kod = "FTR", Onek = "FTR", Yil = 2026, SonNumara = 0, Uzunluk = (byte)6, KayitTarihi = tarih, SilindiMi = false }, // Fatura
            new { Id = 5, Kod = "THS", Onek = "THS", Yil = 2026, SonNumara = 0, Uzunluk = (byte)6, KayitTarihi = tarih, SilindiMi = false }); // Tahsilat
    }
}
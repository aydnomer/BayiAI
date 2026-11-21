using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BayiAI.Data.Configurations;

public class SiparisConfiguration : IEntityTypeConfiguration<Siparis>
{
    public void Configure(EntityTypeBuilder<Siparis> b)
    {
        b.ToTable("Siparis");
        b.Property(x => x.SiparisNo).HasMaxLength(20).IsRequired();
        b.Property(x => x.Aciklama).HasMaxLength(500);
        b.HasIndex(x => x.SiparisNo).IsUnique();
        b.HasIndex(x => x.SiparisTarihi);

        b.HasOne(x => x.Bayi).WithMany(x => x.Siparisler).HasForeignKey(x => x.BayiId);
        b.HasOne<Kullanici>().WithMany().HasForeignKey(x => x.KullaniciId);

        // Kalemler dışarıya salt okunur; EF Core listeyi private _kalemler alanı üzerinden doldurur.
        b.Navigation(x => x.Kalemler).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class SiparisKalemiConfiguration : IEntityTypeConfiguration<SiparisKalemi>
{
    public void Configure(EntityTypeBuilder<SiparisKalemi> b)
    {
        b.ToTable("SiparisKalemi", t => t.HasCheckConstraint("CK_SiparisKalemi_Miktar", "[Miktar] > 0"));
        b.Property(x => x.IskontoOrani).HasPrecision(5, 2);
        b.Property(x => x.KdvOrani).HasPrecision(5, 2);

        // Aynı ürün bir siparişte iki kez olamaz.
        b.HasIndex(x => new { x.SiparisId, x.UrunId }).IsUnique();

        b.HasOne(x => x.Urun).WithMany().HasForeignKey(x => x.UrunId);
    }
}

public class SevkiyatConfiguration : IEntityTypeConfiguration<Sevkiyat>
{
    public void Configure(EntityTypeBuilder<Sevkiyat> b)
    {
        b.ToTable("Sevkiyat");
        b.Property(x => x.AracPlaka).HasMaxLength(15);
        b.Property(x => x.KargoFirmasi).HasMaxLength(100);
        b.Property(x => x.TakipNo).HasMaxLength(50);
        b.HasOne(x => x.Siparis).WithMany().HasForeignKey(x => x.SiparisId);
    }
}

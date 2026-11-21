using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BayiAI.Data.Configurations;

public class UrunConfiguration : IEntityTypeConfiguration<Urun>
{
    public void Configure(EntityTypeBuilder<Urun> b)
    {
        b.ToTable("Urun", t =>
        {
            t.HasCheckConstraint("CK_Urun_Fiyat", "[ListeFiyati] >= 0");
            t.HasCheckConstraint("CK_Urun_Stok", "[StokMiktari] >= 0");   // stok eksiye düşemez
        });

        b.Property(x => x.UrunKodu).HasMaxLength(30).IsRequired();
        b.Property(x => x.Ad).HasMaxLength(200).IsRequired();
        b.Property(x => x.Birim).HasMaxLength(20).IsRequired();
        b.Property(x => x.KdvOrani).HasPrecision(5, 2);
        b.HasIndex(x => x.UrunKodu).IsUnique();

        b.HasOne(x => x.Kategori).WithMany(x => x.Urunler).HasForeignKey(x => x.KategoriId);
    }
}

public class StokHareketConfiguration : IEntityTypeConfiguration<StokHareket>
{
    public void Configure(EntityTypeBuilder<StokHareket> b)
    {
        b.ToTable("StokHareket");
        b.Property(x => x.Aciklama).HasMaxLength(300);
        b.HasOne(x => x.Urun).WithMany().HasForeignKey(x => x.UrunId);
        b.HasOne(x => x.Sevkiyat).WithMany().HasForeignKey(x => x.SevkiyatId);
        b.HasOne<Kullanici>().WithMany().HasForeignKey(x => x.KullaniciId);
    }
}

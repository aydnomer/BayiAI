using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BayiAI.Data.Configurations;

public class KullaniciConfiguration : IEntityTypeConfiguration<Kullanici>
{
    public void Configure(EntityTypeBuilder<Kullanici> b)
    {
        b.ToTable("Kullanici");
        b.Property(x => x.AdSoyad).HasMaxLength(100).IsRequired();
        b.Property(x => x.KullaniciAdi).HasMaxLength(50).IsRequired();
        b.Property(x => x.SifreHash).HasMaxLength(256).IsRequired();
        b.HasIndex(x => x.KullaniciAdi).IsUnique();
    }
}

public class BolgeConfiguration : IEntityTypeConfiguration<Bolge>
{
    public void Configure(EntityTypeBuilder<Bolge> b)
    {
        b.ToTable("Bolge");
        b.Property(x => x.Ad).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.Ad).IsUnique();
    }
}

public class KategoriConfiguration : IEntityTypeConfiguration<Kategori>
{
    public void Configure(EntityTypeBuilder<Kategori> b)
    {
        b.ToTable("Kategori");
        b.Property(x => x.Ad).HasMaxLength(100).IsRequired();

        // Kendine bağlanan ilişki (self-reference)
        b.HasOne(x => x.UstKategori)
         .WithMany(x => x.AltKategoriler)
         .HasForeignKey(x => x.UstKategoriId);
    }
}

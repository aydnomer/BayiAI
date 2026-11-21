using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BayiAI.Data.Configurations;

public class BayiConfiguration : IEntityTypeConfiguration<Bayi>
{
    public void Configure(EntityTypeBuilder<Bayi> b)
    {
        b.ToTable("Bayi", t =>
        {
            t.HasCheckConstraint("CK_Bayi_Iskonto", "[IskontoOrani] BETWEEN 0 AND 50");
            t.HasCheckConstraint("CK_Bayi_Risk", "[RiskLimiti] >= 0");
        });

        b.Property(x => x.BayiKodu).HasMaxLength(20).IsRequired();
        b.Property(x => x.Unvan).HasMaxLength(200).IsRequired();
        b.Property(x => x.VergiNo).HasMaxLength(11).IsRequired();
        b.Property(x => x.VergiDairesi).HasMaxLength(100);
        b.Property(x => x.YetkiliAdSoyad).HasMaxLength(100);
        b.Property(x => x.Telefon).HasMaxLength(20);
        b.Property(x => x.Eposta).HasMaxLength(100);
        b.Property(x => x.Adres).HasMaxLength(400);
        b.Property(x => x.IskontoOrani).HasPrecision(5, 2);

        b.HasIndex(x => x.BayiKodu).IsUnique();
        b.HasIndex(x => x.VergiNo).IsUnique();

        b.HasOne(x => x.Bolge).WithMany(x => x.Bayiler).HasForeignKey(x => x.BolgeId);
        b.HasOne(x => x.SorumluKullanici).WithMany().HasForeignKey(x => x.SorumluKullaniciId);
    }
}

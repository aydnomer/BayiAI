using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BayiAI.Data.Configurations;

public class FaturaConfiguration : IEntityTypeConfiguration<Fatura>
{
    public void Configure(EntityTypeBuilder<Fatura> b)
    {
        b.ToTable("Fatura");
        b.Property(x => x.FaturaNo).HasMaxLength(20).IsRequired();
        b.HasIndex(x => x.FaturaNo).IsUnique();
        b.HasIndex(x => x.VadeTarihi);

        // Bire-bir: bir siparişin tek faturası olur (EF, SiparisId üzerinde UNIQUE index oluşturur).
        b.HasOne(x => x.Siparis).WithOne().HasForeignKey<Fatura>(x => x.SiparisId);
        b.HasOne(x => x.Bayi).WithMany().HasForeignKey(x => x.BayiId);
    }
}

public class KasaBankaConfiguration : IEntityTypeConfiguration<KasaBanka>
{
    public void Configure(EntityTypeBuilder<KasaBanka> b)
    {
        b.ToTable("KasaBanka");
        b.Property(x => x.Ad).HasMaxLength(100).IsRequired();
        b.Property(x => x.Iban).HasMaxLength(34);
        b.HasIndex(x => x.Ad).IsUnique();
    }
}

public class TahsilatConfiguration : IEntityTypeConfiguration<Tahsilat>
{
    public void Configure(EntityTypeBuilder<Tahsilat> b)
    {
        b.ToTable("Tahsilat", t => t.HasCheckConstraint("CK_Tahsilat_Tutar", "[Tutar] > 0"));
        b.Property(x => x.Aciklama).HasMaxLength(300);
        b.HasOne(x => x.Bayi).WithMany().HasForeignKey(x => x.BayiId);
        b.HasOne(x => x.KasaBanka).WithMany().HasForeignKey(x => x.KasaBankaId);
        b.HasOne<Kullanici>().WithMany().HasForeignKey(x => x.KullaniciId);
    }
}

public class CariHareketConfiguration : IEntityTypeConfiguration<CariHareket>
{
    public void Configure(EntityTypeBuilder<CariHareket> b)
    {
        b.ToTable("CariHareket", t => t.HasCheckConstraint(
            "CK_CariHareket_Tutar", "[Borc] >= 0 AND [Alacak] >= 0 AND ([Borc] > 0 OR [Alacak] > 0)"));
        b.Property(x => x.Aciklama).HasMaxLength(300);

        // Bakiye sorgusu (SUM(Borc) - SUM(Alacak)) bu index sayesinde tabloya hiç dokunmadan çalışır.
        b.HasIndex(x => x.BayiId).IncludeProperties(x => new { x.Borc, x.Alacak });

        b.HasOne(x => x.Bayi).WithMany(x => x.CariHareketler).HasForeignKey(x => x.BayiId);
        b.HasOne(x => x.Fatura).WithMany().HasForeignKey(x => x.FaturaId);
        b.HasOne(x => x.Tahsilat).WithMany().HasForeignKey(x => x.TahsilatId);
    }
}

using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BayiAI.Data.Configurations;

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

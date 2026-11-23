using System.Linq.Expressions;
using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BayiAI.Data;

public class BayiDbContext : DbContext
{
    public BayiDbContext(DbContextOptions<BayiDbContext> options) : base(options) { }

    // Tanımlar
    public DbSet<Kullanici> Kullanicilar => Set<Kullanici>();
    public DbSet<Bolge> Bolgeler => Set<Bolge>();
    public DbSet<Kategori> Kategoriler => Set<Kategori>();

    // Bayi
    public DbSet<Bayi> Bayiler => Set<Bayi>();

    // Ürün ve stok
    public DbSet<Urun> Urunler => Set<Urun>();
    public DbSet<StokHareket> StokHareketleri => Set<StokHareket>();

    // Satış
    public DbSet<Siparis> Siparisler => Set<Siparis>();
    public DbSet<SiparisKalemi> SiparisKalemleri => Set<SiparisKalemi>();
    public DbSet<Sevkiyat> Sevkiyatlar => Set<Sevkiyat>();

    // Muhasebe
    public DbSet<Fatura> Faturalar => Set<Fatura>();
    public DbSet<KasaBanka> KasaBankalar => Set<KasaBanka>();
    public DbSet<Tahsilat> Tahsilatlar => Set<Tahsilat>();
    public DbSet<CariHareket> CariHareketler => Set<CariHareket>();

    // Sistem
    public DbSet<IslemLog> IslemLoglari => Set<IslemLog>();
    public DbSet<AiIslem> AiIslemleri => Set<AiIslem>();

    // DbSet: C# tarafında tabloya erişim kapısı. db.FirmaAyarlari ile FirmaAyar tablosuna ulaşırız.
    public DbSet<FirmaAyar> FirmaAyarlari => Set<FirmaAyar>();
    public DbSet<NumaraSerisi> NumaraSerileri => Set<NumaraSerisi>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        // Tüm para alanları varsayılan olarak decimal(18,2).
        // float/double kuruşu yuvarlayıp hatalı hesap çıkarır, bu yüzden hiç kullanmıyoruz.
        builder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurations klasöründeki tüm IEntityTypeConfiguration sınıflarını uygular.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BayiDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // 1) SQL Server, bir tabloya birden fazla yoldan zincirleme silme (cascade) gelmesine
            //    izin vermez. Varsayılanı "Restrict" yapıyoruz: ilişkili kayıt varken silinemez.
            foreach (var fk in entityType.GetForeignKeys())
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            // 2) Ortak kolonlara SQL varsayılanı: SQL script'iyle eklenen örnek veriler de çalışsın.
            var kayitTarihi = entityType.FindProperty(nameof(BaseEntity.KayitTarihi));
            kayitTarihi?.SetDefaultValueSql("SYSDATETIME()");
            var silindiMi = entityType.FindProperty(nameof(BaseEntity.SilindiMi));
            silindiMi?.SetDefaultValue(false);

            // 3) Soft delete: SilindiMi = true olan kayıtlar sorgulara hiç gelmez.
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var e = Expression.Parameter(entityType.ClrType, "e");
                var filtre = Expression.Lambda(
                    Expression.Not(Expression.Property(e, nameof(BaseEntity.SilindiMi))), e);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filtre);
            }
        }

        // Tek istisna: sipariş silinirse kalemleri de silinsin.
        modelBuilder.Entity<SiparisKalemi>()
            .HasOne(k => k.Siparis)
            .WithMany(s => s.Kalemler)
            .HasForeignKey(k => k.SiparisId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

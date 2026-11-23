using System.Linq.Expressions;
using BayiAI.Core.Entities;
using BayiAI.Core.Interfaces;
// EF Core'un FirstOrDefaultAsync, AnyAsync gibi metotları buradan gelir.
using Microsoft.EntityFrameworkCore;

namespace BayiAI.Data.Repositories;

// IRepository<T> arayüzündeki sözü yerine getiren (implement eden) sınıf.
// Bir kez yazılır, 15 tablonun hepsi için çalışır.
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    // protected: Bu sınıf ve ondan türeyen sınıflar görebilir.
    // Db: Veritabanı bağlantımız. Set: T tablosu (örn. Kullanicilar tablosu).
    protected readonly BayiDbContext Db;
    protected readonly DbSet<T> Set;

    // Constructor (yapıcı): new Repository<Kullanici>(db) yazılınca çalışır.
    public Repository(BayiDbContext db)
    {
        Db = db;
        // db.Set<T>(): T hangi sınıfsa onun tablosunu verir.
        Set = db.Set<T>();
    }

    // Id'si eşleşen ilk kaydı getirir. SQL karşılığı: SELECT TOP 1 * FROM ... WHERE Id = @id
    // virtual: Türeyen sınıflar bu metodu değiştirebilir (override).
    public virtual async Task<T?> GetirAsync(int id) =>
        await Set.FirstOrDefaultAsync(x => x.Id == id);

    // AsNoTracking: EF bu kayıtları takip etmez. Sadece okuyacaksak daha hızlıdır.
    // filtre is null ? A : B → filtre boşsa A'yı, doluysa B'yi çalıştır (üçlü operatör).
    public virtual async Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null) =>
        filtre is null
            ? await Set.AsNoTracking().ToListAsync()
            : await Set.AsNoTracking().Where(filtre).ToListAsync();

    // Filtreye uyan ilk kayıt. AsNoTracking YOK, çünkü bu kaydı değiştirip kaydedeceğiz.
    // EF kaydı takip eder, KaydetAsync dediğimizde değişen kolonları UPDATE eder.
    public virtual async Task<T?> BulAsync(Expression<Func<T, bool>> filtre) =>
        await Set.FirstOrDefaultAsync(filtre);

    // SQL karşılığı: SELECT CASE WHEN EXISTS (SELECT ... WHERE ...) THEN 1 ELSE 0 END
    public async Task<bool> VarMiAsync(Expression<Func<T, bool>> filtre) =>
        await Set.AnyAsync(filtre);

    // Yeni kaydı EF'in "eklenecekler" listesine koyar.
    public async Task EkleAsync(T entity) => await Set.AddAsync(entity);

    // Kaydı "güncellenecek" olarak işaretler.
    public void Guncelle(T entity) => Set.Update(entity);

    // Soft delete: BaseEntity'deki Sil() metodu SilindiMi = true yapar, sonra güncellenir.
    public void Sil(T entity)
    {
        entity.Sil();
        Set.Update(entity);
    }

    // Bekleyen tüm ekleme/güncelleme işlemlerini tek seferde veritabanına yazar.
    public Task<int> KaydetAsync() => Db.SaveChangesAsync();
}

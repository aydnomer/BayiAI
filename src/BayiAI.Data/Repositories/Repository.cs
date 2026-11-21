using System.Linq.Expressions;
using BayiAI.Core.Entities;
using BayiAI.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BayiAI.Data.Repositories;

/// <summary>
/// IRepository&lt;T&gt; arayüzünün EF Core ile genel (generic) uygulaması.
/// Bir kez yazılır, 15 tablonun hepsinde kullanılır.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly BayiDbContext Db;
    protected readonly DbSet<T> Set;

    public Repository(BayiDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public virtual async Task<T?> GetirAsync(int id) =>
        await Set.FirstOrDefaultAsync(x => x.Id == id);

    public virtual async Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null) =>
        filtre is null
            ? await Set.AsNoTracking().ToListAsync()
            : await Set.AsNoTracking().Where(filtre).ToListAsync();

    public async Task EkleAsync(T entity) => await Set.AddAsync(entity);

    public void Guncelle(T entity) => Set.Update(entity);

    public void Sil(T entity)
    {
        entity.Sil();          // SilindiMi = true (soft delete)
        Set.Update(entity);
    }

    public Task<int> KaydetAsync() => Db.SaveChangesAsync();
}

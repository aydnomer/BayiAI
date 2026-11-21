using BayiAI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BayiAI.Data.Repositories;

public class CariRepository : Repository<CariHareket>
{
    public CariRepository(BayiDbContext db) : base(db) { }

    /// <summary>
    /// Bayinin güncel bakiyesi. SQL karşılığı:
    /// SELECT SUM(Borc) - SUM(Alacak) FROM CariHareket WHERE BayiId = @bayiId
    /// </summary>
    public async Task<decimal> BakiyeGetirAsync(int bayiId) =>
        await Set.Where(c => c.BayiId == bayiId)
                 .SumAsync(c => (decimal?)(c.Borc - c.Alacak)) ?? 0m;

    /// <summary>Bayinin cari ekstresi: tarih sırasına göre tüm hareketler.</summary>
    public Task<List<CariHareket>> EkstreGetirAsync(int bayiId, DateTime baslangic, DateTime bitis) =>
        Set.AsNoTracking()
           .Where(c => c.BayiId == bayiId && c.Tarih >= baslangic && c.Tarih <= bitis)
           .OrderBy(c => c.Tarih)
           .ToListAsync();
}

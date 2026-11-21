using BayiAI.Core.Entities;
using BayiAI.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace BayiAI.Data.Repositories;

public class SiparisRepository : Repository<Siparis>
{
    public SiparisRepository(BayiDbContext db) : base(db) { }

    /// <summary>Siparişi bayisi, kalemleri ve kalemlerin ürünleriyle birlikte getirir (JOIN).</summary>
    public Task<Siparis?> KalemleriyleGetirAsync(int siparisId) =>
        Set.Include(s => s.Bayi)
           .Include(s => s.Kalemler).ThenInclude(k => k.Urun)
           .FirstOrDefaultAsync(s => s.Id == siparisId);

    public Task<List<Siparis>> DurumaGoreListeleAsync(SiparisDurumu durum) =>
        Set.AsNoTracking()
           .Include(s => s.Bayi)
           .Where(s => s.Durum == durum)
           .OrderByDescending(s => s.SiparisTarihi)
           .ToListAsync();
}

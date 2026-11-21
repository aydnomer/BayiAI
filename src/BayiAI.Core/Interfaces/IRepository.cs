using System.Linq.Expressions;
using BayiAI.Core.Entities;

namespace BayiAI.Core.Interfaces;

/// <summary>
/// Tüm tablolar için ortak veri erişim sözleşmesi (generic + interface).
/// Somut uygulaması BayiAI.Data projesindedir; iş katmanı yalnızca bu arayüzü tanır.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetirAsync(int id);
    Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null);
    Task EkleAsync(T entity);
    void Guncelle(T entity);
    void Sil(T entity);           // soft delete: SilindiMi = true
    Task<int> KaydetAsync();
}

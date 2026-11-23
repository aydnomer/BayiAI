// Expression: Filtreyi (x => x.KullaniciAdi == "ali" gibi) kod olarak taşımamızı sağlar.
// EF Core bunu SQL'deki WHERE kısmına çevirir.
using System.Linq.Expressions;
using BayiAI.Core.Entities;

namespace BayiAI.Core.Interfaces;

// interface (arayüz): Sadece "ne yapılacağını" söyler, "nasıl" yapılacağını söylemez.
// Asıl kod BayiAI.Data projesindeki Repository sınıfında.
// <T>: Generic. T yerine Bayi, Urun, Kullanici... hangi tablo gelirse onunla çalışır.
// where T : BaseEntity: T sadece BaseEntity'den türeyen sınıflar olabilir.
public interface IRepository<T> where T : BaseEntity
{
    // Id'ye göre tek kayıt getirir. Bulamazsa null döner (T? bu yüzden).
    // Task: Async (beklemeli) çalışır, veritabanını beklerken program donmaz.
    Task<T?> GetirAsync(int id);

    // Kayıtları liste olarak getirir. Filtre verilmezse hepsini getirir.
    Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null);

    // Filtreye uyan İLK kaydı getirir. Bu kayıt üzerinde değişiklik yapıp kaydedebiliriz.
    // Örnek: BulAsync(x => x.KullaniciAdi == "admin")
    Task<T?> BulAsync(Expression<Func<T, bool>> filtre);

    // Filtreye uyan kayıt var mı? Sadece true/false döner, kaydı getirmez (hızlıdır).
    // Örnek: VarMiAsync(x => x.VergiNo == "1234567890") → aynı vergi no var mı?
    Task<bool> VarMiAsync(Expression<Func<T, bool>> filtre);

    // Yeni kayıt ekler (henüz veritabanına yazmaz, KaydetAsync bekler).
    Task EkleAsync(T entity);

    // Kaydı güncellenecek olarak işaretler.
    void Guncelle(T entity);

    // Soft delete: Kaydı silmez, SilindiMi = true yapar.
    void Sil(T entity);

    // Bekleyen tüm değişiklikleri veritabanına yazar. Kaç satır etkilendiğini döndürür.
    Task<int> KaydetAsync();
}

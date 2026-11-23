using System.Linq.Expressions;
using BayiAI.Core.Entities;
using BayiAI.Core.Interfaces;

namespace BayiAI.Tests;

// SAHTE REPOSITORY: Gerçek veritabanı yerine bellekteki bir List<T> kullanır.
// Servislerimiz sadece IRepository arayüzünü tanıdığı için gerçeğinin yerine bunu verebiliyoruz.
// "Arayüze göre programlama" ve "bağımlılık enjeksiyonu"nun faydası tam olarak bu:
// Test hızlı çalışır, veritabanına dokunmaz, her seferinde temiz başlar.
public class SahteRepository<T> : IRepository<T> where T : BaseEntity
{
    // Veritabanı tablosu yerine geçen liste.
    public List<T> Kayitlar { get; } = new();

    // KaydetAsync kaç kez çağrıldı? (Gerekirse testte kontrol etmek için.)
    public int KaydetSayisi { get; private set; }

    // Task.FromResult: Async metot gibi davranan ama hemen sonuç veren görev.
    public Task<T?> GetirAsync(int id) => Task.FromResult(Kayitlar.FirstOrDefault(x => x.Id == id));

    // filtre.Compile(): Expression'ı (SQL'e çevrilecek ifade) normal C# fonksiyonuna çevirir, listede çalıştırabilelim diye.
    public Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null) =>
        Task.FromResult(filtre is null ? Kayitlar.ToList() : Kayitlar.Where(filtre.Compile()).ToList());

    public Task<T?> BulAsync(Expression<Func<T, bool>> filtre) =>
        Task.FromResult(Kayitlar.FirstOrDefault(filtre.Compile()));

    public Task<bool> VarMiAsync(Expression<Func<T, bool>> filtre) =>
        Task.FromResult(Kayitlar.Any(filtre.Compile()));

    public Task EkleAsync(T entity)
    {
        Kayitlar.Add(entity);
        return Task.CompletedTask;   // "İş bitti" anlamında boş görev
    }

    // Bellekteki nesne zaten güncel, yapacak bir şey yok.
    public void Guncelle(T entity) { }

    public void Sil(T entity) => entity.Sil();

    public Task<int> KaydetAsync()
    {
        KaydetSayisi++;
        return Task.FromResult(1);
    }
}

// SAHTE SAAT: Testte zamanı biz kontrol ederiz. "15 dakika bekle" testini gerçekten
// 15 dakika beklemeden, saati ileri alarak yaparız.
// : TimeProvider → .NET'in saat sınıfından miras alır, metotlarını ezer (override).
public class SabitSaat : TimeProvider
{
    private DateTimeOffset _simdi;

    // Başlangıç zamanını biz veririz.
    public SabitSaat(DateTime baslangic) => _simdi = new DateTimeOffset(baslangic, TimeSpan.Zero);

    // override: Üst sınıftaki metodu kendi versiyonumuzla değiştiriyoruz. Her zaman bizim saatimizi döndür.
    public override DateTimeOffset GetUtcNow() => _simdi;
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

    // Saati ileri al.
    public void Ilerlet(TimeSpan sure) => _simdi = _simdi.Add(sure);
}
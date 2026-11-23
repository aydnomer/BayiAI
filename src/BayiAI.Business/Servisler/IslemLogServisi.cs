using BayiAI.Business.Guvenlik;
using BayiAI.Core.Entities;
using BayiAI.Core.Interfaces;

namespace BayiAI.Business.Servisler;

// "Kim, ne zaman, hangi kayıtta ne yaptı?" bilgisini IslemLog tablosuna yazar.
// : ServisBase → ServisBase'den miras alır (Oturum, Simdi gibi şeyleri hazır kullanır).
public class IslemLogServisi : ServisBase
{
    // _ ile başlayan isim: Sınıfa özel (private) alan olduğunu gösteren yaygın bir alışkanlık.
    private readonly IRepository<IslemLog> _loglar;

    // Bu servis oluşturulurken log tablosunun deposu, oturum ve (isteğe bağlı) saat verilir.
    // TimeProvider? saat = null: Bu parametre verilmeyebilir, verilmezse null.
    // : base(oturum, saat) → ServisBase'in constructor'ını çağırır.
    public IslemLogServisi(IRepository<IslemLog> loglar, OturumBilgisi oturum, TimeProvider? saat = null)
        : base(oturum, saat)
    {
        _loglar = loglar;
    }

    // Bir log satırı ekler ve hemen kaydeder.
    // tabloAdi: "Bayi", kayitId: 5, islem: "Guncelle" gibi. Eski/yeni değer isteğe bağlı.
    public async Task YazAsync(string tabloAdi, int kayitId, string islem, string? eskiDeger = null, string? yeniDeger = null)
    {
        // new IslemLog { ... }: Nesne oluşturup özelliklerini aynı anda doldurma (object initializer).
        await _loglar.EkleAsync(new IslemLog
        {
            KullaniciId = Oturum.KullaniciId,   // İşlemi yapan kişi
            Tarih = Simdi,                      // Ne zaman
            TabloAdi = tabloAdi,                // Hangi tablo
            KayitId = kayitId,                  // Hangi kayıt
            Islem = islem,                      // Ne yapıldı
            EskiDeger = eskiDeger,              // Önceki hali
            YeniDeger = yeniDeger               // Sonraki hali
        });
        await _loglar.KaydetAsync();
    }

    // İki tarih arasındaki logları getirir. Tablo adı verilirse sadece o tablonun loglarını.
    // (tabloAdi == null || ...) → tablo adı verilmediyse bu şartı atla demek.
    public Task<List<IslemLog>> ListeleAsync(DateTime baslangic, DateTime bitis, string? tabloAdi = null) =>
        _loglar.ListeleAsync(x => x.Tarih >= baslangic && x.Tarih < bitis
                                  && (tabloAdi == null || x.TabloAdi == tabloAdi));
}

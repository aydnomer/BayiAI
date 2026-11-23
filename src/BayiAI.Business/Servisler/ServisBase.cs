using BayiAI.Business.Guvenlik;
using BayiAI.Core.Enums;

namespace BayiAI.Business.Servisler;

// abstract (soyut): Bu sınıftan direkt nesne üretilemez, sadece miras alınır.
// Tüm servislerin (Giris, Bayi, Urun...) ortak atası. Ortak kodu bir kez burada yazıyoruz.
public abstract class ServisBase
{
    // Programı şu an kim kullanıyor? Tüm servisler bunu bilmeli.
    protected readonly OturumBilgisi Oturum;

    // TimeProvider: .NET'in "saat" sınıfı. Neden DateTime.Now kullanmıyoruz?
    // Çünkü testlerde zamanı biz belirleyebilelim diye (örn. "15 dakika sonra kilit açılıyor mu?" testi).
    protected readonly TimeProvider Saat;

    // protected constructor: Sadece türeyen sınıflar çağırabilir.
    // TimeProvider? saat: Verilmezse null gelir.
    protected ServisBase(OturumBilgisi oturum, TimeProvider? saat)
    {
        Oturum = oturum;
        // ?? operatörü: Soldaki null ise sağdakini kullan. Saat verilmediyse gerçek sistem saati.
        Saat = saat ?? TimeProvider.System;
    }

    // Şu anki yerel saat. Servislerde DateTime.Now yerine hep bunu kullanacağız.
    protected DateTime Simdi => Saat.GetLocalNow().DateTime;

    // Yetki kontrolü: Yetki varsa null, yoksa hata mesajı döner.
    // Kullanımı: var hata = YetkiHatasi(Yetki.BayiDuzenle); if (hata != null) return Hata(hata);
    protected string? YetkiHatasi(Yetki yetki) =>
        Oturum.YetkiVarMi(yetki) ? null : "Bu işlem için yetkiniz yok.";
}

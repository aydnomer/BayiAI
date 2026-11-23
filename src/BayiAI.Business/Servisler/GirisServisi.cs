using BayiAI.Business.Guvenlik;
using BayiAI.Core;             // IslemSonucu burada
using BayiAI.Core.Entities;
using BayiAI.Core.Interfaces;

namespace BayiAI.Business.Servisler;

// Giriş ve çıkış işlemleri. Giriş ekranı sadece bu sınıfla konuşacak.
public class GirisServisi : ServisBase
{
    // GÜVENLİK: "Böyle kullanıcı yok" ve "şifre yanlış" için AYNI mesajı veriyoruz.
    // Farklı mesaj verirsek saldırgan hangi kullanıcı adlarının var olduğunu öğrenir.
    private const string GenelHata = "Kullanıcı adı veya şifre hatalı.";

    private readonly IRepository<Kullanici> _kullanicilar;
    private readonly IslemLogServisi _log;

    // Dependency Injection (bağımlılık enjeksiyonu): İhtiyaç duyduğu nesneleri kendisi üretmez,
    // dışarıdan alır. Böylece testte sahte depo verebiliriz.
    public GirisServisi(IRepository<Kullanici> kullanicilar, IslemLogServisi log,
                        OturumBilgisi oturum, TimeProvider? saat = null)
        : base(oturum, saat)
    {
        _kullanicilar = kullanicilar;
        _log = log;
    }

    // Giriş denemesi. Sonuç olarak IslemSonucu döner: Basarili, Veri (kullanıcı), Mesaj.
    public async Task<IslemSonucu<Kullanici>> GirisYapAsync(string kullaniciAdi, string sifre)
    {
        // 1) Boş alan kontrolü.
        if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            return IslemSonucu<Kullanici>.Hata("Kullanıcı adı ve şifre girin.");

        // Zamanı bir kez alıyoruz, tüm kontroller aynı anı kullansın.
        var simdi = Simdi;

        // Trim: Baştaki ve sondaki boşlukları siler (" admin " → "admin").
        var ad = kullaniciAdi.Trim();

        // 2) Kullanıcıyı veritabanında bul.
        var kullanici = await _kullanicilar.BulAsync(x => x.KullaniciAdi == ad);

        // Yoksa genel hata (yukarıdaki güvenlik notu).
        if (kullanici is null)
            return IslemSonucu<Kullanici>.Hata(GenelHata);

        // 3) Hesap pasifse giremez.
        if (!kullanici.AktifMi)
            return IslemSonucu<Kullanici>.Hata("Hesabınız pasif durumda. Yöneticinize başvurun.");

        // 4) Hesap kilitliyse kaç dakika kaldığını söyle.
        if (kullanici.KilitliMi(simdi))
            return IslemSonucu<Kullanici>.Hata(KilitMesaji(kullanici, simdi));

        // 5) Şifre yanlışsa...
        if (!SifreHasher.Dogrula(sifre, kullanici.SifreHash))
        {
            // ...hatalı deneme sayısını artır (5 olursa Kullanici sınıfı kendini kilitler).
            kullanici.HataliGirisKaydet(simdi);
            await _kullanicilar.KaydetAsync();

            // Bu deneme ile kilitlendiyse kilit mesajı, değilse genel hata.
            return kullanici.KilitliMi(simdi)
                ? IslemSonucu<Kullanici>.Hata(KilitMesaji(kullanici, simdi))
                : IslemSonucu<Kullanici>.Hata(GenelHata);
        }

        // 6) Buraya geldiysek şifre doğru: sayacı sıfırla, son giriş zamanını yaz.
        kullanici.BasariliGirisKaydet(simdi);
        await _kullanicilar.KaydetAsync();

        // Oturumu başlat (artık program bu kişinin kim olduğunu biliyor) ve log yaz.
        Oturum.Baslat(kullanici, simdi);
        await _log.YazAsync("Kullanici", kullanici.Id, "Giris");

        // Şifre değiştirmesi gerekiyorsa uyarı, değilse karşılama mesajı.
        string mesaj = kullanici.SifreDegistirmeli
            ? "Güvenliğiniz için şifrenizi değiştirmeniz gerekiyor."
            : $"Hoş geldiniz, {kullanici.AdSoyad}.";
        return IslemSonucu<Kullanici>.Basari(kullanici, mesaj);
    }

    // Çıkış: Log yaz, oturumu kapat.
    public async Task CikisYapAsync()
    {
        // "is int id" → KullaniciId doluysa değerini id değişkenine koy ve içeri gir (pattern matching).
        if (Oturum.KullaniciId is int id)
            await _log.YazAsync("Kullanici", id, "Cikis");
        Oturum.Kapat();
    }

    // Kilidin bitmesine kaç dakika kaldığını hesaplayıp mesaj üretir.
    // static: Nesnenin alanlarını kullanmadığı için static olabilir.
    private static string KilitMesaji(Kullanici k, DateTime simdi)
    {
        // Kilit bitişi - şimdi = kalan süre. Math.Ceiling yukarı yuvarlar (4,2 dk → 5 dk).
        // ! işareti: "Bu değer null değil, eminim" demek (null-forgiving).
        int dakika = (int)Math.Ceiling((k.KilitBitisTarihi!.Value - simdi).TotalMinutes);
        return $"Çok fazla hatalı deneme yapıldı. Hesap {dakika} dakika kilitli.";
    }
}

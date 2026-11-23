using BayiAI.Business.Guvenlik;
using BayiAI.Core;
using BayiAI.Core.Entities;
using BayiAI.Core.Enums;
using BayiAI.Core.Exceptions;
using BayiAI.Core.Interfaces;

namespace BayiAI.Business.Servisler;

// Ürün kartı ve stok girişi işlemleri.
public class UrunServisi : ServisBase
{
    // Türkiye'de geçerli KDV oranları. Bunların dışında bir oran girilemez.
    public static readonly decimal[] GecerliKdvOranlari = { 0, 1, 10, 20 };

    private readonly IRepository<Urun> _urunler;
    private readonly IRepository<StokHareket> _stokHareketleri;
    private readonly IslemLogServisi _log;

    public UrunServisi(IRepository<Urun> urunler, IRepository<StokHareket> stokHareketleri,
                       IslemLogServisi log, OturumBilgisi oturum, TimeProvider? saat = null)
        : base(oturum, saat)
    {
        _urunler = urunler;
        _stokHareketleri = stokHareketleri;
        _log = log;
    }

    // Ürün listesi: ad veya kodda arama, kategoriye göre filtre.
    public async Task<IslemSonucu<List<Urun>>> ListeleAsync(string? arama = null, int? kategoriId = null)
    {
        if (YetkiHatasi(Yetki.UrunGoruntule) is string hata) return IslemSonucu<List<Urun>>.Hata(hata);

        var aranan = string.IsNullOrWhiteSpace(arama) ? null : arama.Trim();
        var liste = await _urunler.ListeleAsync(x =>
            (aranan == null || x.Ad.Contains(aranan) || x.UrunKodu.Contains(aranan)) &&
            (kategoriId == null || x.KategoriId == kategoriId));

        return IslemSonucu<List<Urun>>.Basari(liste.OrderBy(x => x.Ad).ToList());
    }

    // Stoğu kritik seviyede veya altında olan aktif ürünler. Ana ekranda kırmızı uyarı olarak göstereceğiz.
    public Task<List<Urun>> KritikStokListesiAsync() =>
        _urunler.ListeleAsync(x => x.AktifMi && x.StokMiktari <= x.KritikStok);

    // Ürün ekle (Id = 0) veya güncelle.
    public async Task<IslemSonucu<Urun>> KaydetAsync(Urun urun)
    {
        if (YetkiHatasi(Yetki.UrunDuzenle) is string yetkiHatasi) return IslemSonucu<Urun>.Hata(yetkiHatasi);

        // Ürün kodunu büyük harfe çevir: "su-05" → "SU-05". InvariantCulture: Türkçe i/İ sorununu önler.
        urun.UrunKodu = urun.UrunKodu.Trim().ToUpperInvariant();
        urun.Ad = urun.Ad.Trim();

        var hatalar = new List<string>();
        if (urun.UrunKodu.Length == 0) hatalar.Add("Ürün kodu boş olamaz.");
        if (urun.Ad.Length == 0) hatalar.Add("Ürün adı boş olamaz.");
        if (urun.ListeFiyati <= 0) hatalar.Add("Liste fiyatı sıfırdan büyük olmalı.");
        if (!GecerliKdvOranlari.Contains(urun.KdvOrani)) hatalar.Add("KDV oranı 0, 1, 10 veya 20 olmalı.");
        if (urun.KritikStok < 0) hatalar.Add("Kritik stok negatif olamaz.");
        if (urun.KategoriId <= 0) hatalar.Add("Kategori seçilmeli.");

        int id = urun.Id;
        if (await _urunler.VarMiAsync(x => x.UrunKodu == urun.UrunKodu && x.Id != id))
            hatalar.Add("Bu ürün kodu kullanılıyor.");

        if (hatalar.Count > 0) return IslemSonucu<Urun>.Hata(string.Join(Environment.NewLine, hatalar));

        if (id == 0)
        {
            await _urunler.EkleAsync(urun);
            await _urunler.KaydetAsync();
            await _log.YazAsync("Urun", urun.Id, "Ekle", yeniDeger: Ozet(urun));
            return IslemSonucu<Urun>.Basari(urun, $"{urun.Ad} eklendi.");
        }

        var eski = (await _urunler.ListeleAsync(x => x.Id == id)).FirstOrDefault();
        if (eski is null) return IslemSonucu<Urun>.Hata("Ürün bulunamadı.");

        _urunler.Guncelle(urun);
        await _urunler.KaydetAsync();
        await _log.YazAsync("Urun", id, "Guncelle", Ozet(eski), Ozet(urun));
        return IslemSonucu<Urun>.Basari(urun, "Ürün güncellendi.");
    }

    // Mal kabulü: Depoya ürün geldi. Stok artar ve StokHareket tablosuna "Giriş" kaydı düşülür.
    public async Task<IslemSonucu<Urun>> StokGirisAsync(int urunId, int miktar, string? aciklama)
    {
        if (YetkiHatasi(Yetki.StokIslem) is string yetkiHatasi) return IslemSonucu<Urun>.Hata(yetkiHatasi);

        // "is not int kullaniciId": KullaniciId boşsa (giriş yoksa) hata ver; doluysa değerini kullaniciId'ye al.
        if (Oturum.KullaniciId is not int kullaniciId) return IslemSonucu<Urun>.Hata("Önce giriş yapın.");

        var urun = await _urunler.GetirAsync(urunId);
        if (urun is null) return IslemSonucu<Urun>.Hata("Ürün bulunamadı.");

        try
        {
            // Stoğu Urun sınıfının kendi metodu artırır (miktar 0 veya negatifse hata fırlatır).
            urun.StokGir(miktar);
        }
        catch (IsKuraliException ex)
        {
            return IslemSonucu<Urun>.Hata(ex.Message);
        }

        // Hareket kaydı: Ne zaman, kim, kaç adet giriş yaptı?
        await _stokHareketleri.EkleAsync(new StokHareket
        {
            UrunId = urunId,
            Tarih = Simdi,
            HareketTipi = StokHareketTipi.Giris,
            Miktar = miktar,
            Aciklama = aciklama,
            KullaniciId = kullaniciId
        });

        // İki repository aynı veritabanı bağlantısını (DbContext) paylaşır. Tek KaydetAsync ikisini birlikte yazar:
        // ya ikisi de kaydedilir ya hiçbiri (transaction). Stok artıp hareket kaydı eksik kalmaz.
        await _urunler.KaydetAsync();
        return IslemSonucu<Urun>.Basari(urun, $"{miktar} {urun.Birim} giriş yapıldı. Yeni stok: {urun.StokMiktari}.");
    }

    private static string Ozet(Urun u) =>
        $"{u.UrunKodu} | {u.Ad} | {u.ListeFiyati:N2} TL | KDV %{u.KdvOrani} | Kritik {u.KritikStok} | {(u.AktifMi ? "Aktif" : "Pasif")}";
}

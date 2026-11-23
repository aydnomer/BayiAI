using BayiAI.Business.Dogrulama;
using BayiAI.Business.Guvenlik;
using BayiAI.Core;
using BayiAI.Core.Entities;
using BayiAI.Core.Enums;
using BayiAI.Core.Exceptions;
using BayiAI.Core.Interfaces;

namespace BayiAI.Business.Servisler;

// Bayi kartı işlemleri. Bayi ekranı sadece bu sınıfla konuşacak, veritabanını hiç bilmeyecek.
public class BayiServisi : ServisBase
{
    private readonly IRepository<Bayi> _bayiler;
    private readonly IRepository<Siparis> _siparisler;   // Silmeden önce açık sipariş var mı bakmak için
    private readonly NumaraServisi _numara;              // Yeni bayiye otomatik kod vermek için
    private readonly IslemLogServisi _log;

    public BayiServisi(IRepository<Bayi> bayiler, IRepository<Siparis> siparisler, NumaraServisi numara,
                       IslemLogServisi log, OturumBilgisi oturum, TimeProvider? saat = null)
        : base(oturum, saat)
    {
        _bayiler = bayiler;
        _siparisler = siparisler;
        _numara = numara;
        _log = log;
    }

    // Bayi listesi. arama: unvan veya kodda geçen metin. bolgeId: sadece o bölge. İkisi de isteğe bağlı.
    public async Task<IslemSonucu<List<Bayi>>> ListeleAsync(string? arama = null, int? bolgeId = null)
    {
        // Yetki yoksa hata mesajını "hata" değişkenine al ve hemen dön.
        if (YetkiHatasi(Yetki.BayiGoruntule) is string hata) return IslemSonucu<List<Bayi>>.Hata(hata);

        var aranan = string.IsNullOrWhiteSpace(arama) ? null : arama.Trim();

        // Contains: SQL'de LIKE '%metin%' olur.
        var liste = await _bayiler.ListeleAsync(x =>
            (aranan == null || x.Unvan.Contains(aranan) || x.BayiKodu.Contains(aranan)) &&
            (bolgeId == null || x.BolgeId == bolgeId));

        // OrderBy: Unvana göre A'dan Z'ye sırala.
        return IslemSonucu<List<Bayi>>.Basari(liste.OrderBy(x => x.Unvan).ToList());
    }

    // Kaydet: Id = 0 ise yeni bayi ekler, değilse mevcut bayiyi günceller.
    public async Task<IslemSonucu<Bayi>> KaydetAsync(Bayi bayi)
    {
        if (YetkiHatasi(Yetki.BayiDuzenle) is string yetkiHatasi) return IslemSonucu<Bayi>.Hata(yetkiHatasi);

        // Kullanıcının yanlışlıkla bıraktığı boşlukları temizle.
        bayi.Unvan = bayi.Unvan.Trim();
        bayi.VergiNo = bayi.VergiNo.Trim();
        bayi.BayiKodu = bayi.BayiKodu.Trim();

        // Tüm hataları bir listede topluyoruz. Kullanıcı formu bir kez gönderip hepsini birden görsün.
        var hatalar = new List<string>();
        if (bayi.Unvan.Length == 0) hatalar.Add("Unvan boş olamaz.");
        if (!Dogrulayici.VergiNoGecerliMi(bayi.VergiNo)) hatalar.Add("Vergi/T.C. kimlik numarası geçersiz.");
        if (!Dogrulayici.EpostaGecerliMi(bayi.Eposta)) hatalar.Add("E-posta adresi geçersiz.");
        if (!Dogrulayici.TelefonGecerliMi(bayi.Telefon)) hatalar.Add("Telefon numarası 10-11 haneli olmalı.");
        if (bayi.BolgeId <= 0) hatalar.Add("Bölge seçilmeli.");

        // Aynı vergi no başka bayide var mı? (x.Id != id: Kendisini sayma, güncellemede gerekli.)
        int id = bayi.Id;
        if (await _bayiler.VarMiAsync(x => x.VergiNo == bayi.VergiNo && x.Id != id))
            hatalar.Add("Bu vergi numarasıyla kayıtlı başka bir bayi var.");
        if (bayi.BayiKodu.Length > 0 && await _bayiler.VarMiAsync(x => x.BayiKodu == bayi.BayiKodu && x.Id != id))
            hatalar.Add("Bu bayi kodu kullanılıyor.");

        // Hata varsa hepsini alt alta birleştirip döndür. Environment.NewLine: Yeni satır karakteri.
        if (hatalar.Count > 0) return IslemSonucu<Bayi>.Hata(string.Join(Environment.NewLine, hatalar));

        // try-catch: Kayıt sırasında bir iş kuralı hatası fırlatılırsa programı çökertme, mesaj olarak döndür.
        try
        {
            if (id == 0)
            {
                // YENİ KAYIT. Kod girilmediyse otomatik ver: B20260001, B20260002...
                if (bayi.BayiKodu.Length == 0)
                    bayi.BayiKodu = await _numara.SonrakiAsync(NumaraServisi.Bayi);

                await _bayiler.EkleAsync(bayi);
                await _bayiler.KaydetAsync();   // Bu satırdan sonra bayi.Id dolar (SQL IDENTITY).
                await _log.YazAsync("Bayi", bayi.Id, "Ekle", yeniDeger: Ozet(bayi));
                return IslemSonucu<Bayi>.Basari(bayi, $"{bayi.Unvan} eklendi.");
            }

            // GÜNCELLEME. Log'a "eski hali" yazabilmek için önce veritabanındaki halini oku.
            var eski = (await _bayiler.ListeleAsync(x => x.Id == id)).FirstOrDefault();
            if (eski is null) return IslemSonucu<Bayi>.Hata("Bayi bulunamadı.");

            _bayiler.Guncelle(bayi);
            await _bayiler.KaydetAsync();
            await _log.YazAsync("Bayi", id, "Guncelle", Ozet(eski), Ozet(bayi));
            return IslemSonucu<Bayi>.Basari(bayi, "Bayi bilgileri güncellendi.");
        }
        catch (IsKuraliException ex)
        {
            return IslemSonucu<Bayi>.Hata(ex.Message);
        }
    }

    // Silme (soft delete). İş kuralı: Açık siparişi olan bayi silinemez.
    public async Task<IslemSonucu<bool>> SilAsync(int bayiId)
    {
        if (YetkiHatasi(Yetki.BayiDuzenle) is string yetkiHatasi) return IslemSonucu<bool>.Hata(yetkiHatasi);

        var bayi = await _bayiler.GetirAsync(bayiId);
        if (bayi is null) return IslemSonucu<bool>.Hata("Bayi bulunamadı.");

        // Bekleyen, onaylı veya yolda olan siparişi var mı?
        bool acikSiparisVar = await _siparisler.VarMiAsync(s => s.BayiId == bayiId &&
            (s.Durum == SiparisDurumu.Beklemede || s.Durum == SiparisDurumu.Onaylandi || s.Durum == SiparisDurumu.SevkEdildi));
        if (acikSiparisVar)
            return IslemSonucu<bool>.Hata("Bayinin tamamlanmamış siparişleri var. Önce onları kapatın veya iptal edin.");

        _bayiler.Sil(bayi);
        await _bayiler.KaydetAsync();
        await _log.YazAsync("Bayi", bayiId, "Sil", Ozet(bayi));
        return IslemSonucu<bool>.Basari(true, $"{bayi.Unvan} silindi.");
    }

    // Log için bayinin tek satırlık özeti. N2: Sayıyı 2 ondalıklı ve binlik ayraçlı yazar (12.500,00).
    private static string Ozet(Bayi b) =>
        $"{b.BayiKodu} | {b.Unvan} | VKN {b.VergiNo} | Bölge {b.BolgeId} | İsk %{b.IskontoOrani} | Risk {b.RiskLimiti:N2} | {(b.AktifMi ? "Aktif" : "Pasif")}";
}
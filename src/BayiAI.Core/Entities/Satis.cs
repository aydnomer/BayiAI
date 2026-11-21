using BayiAI.Core.Enums;
using BayiAI.Core.Exceptions;

namespace BayiAI.Core.Entities;

/// <summary>
/// Bayinin sipariş başlığı. Kalemler dışarıya salt okunur verilir;
/// yeni kalem yalnızca KalemEkle ile eklenir, böylece toplamlar hep doğru kalır.
/// </summary>
public class Siparis : BelgeBase
{
    private readonly List<SiparisKalemi> _kalemler = new();

    public string SiparisNo { get; set; } = "";
    public override string BelgeNo => SiparisNo;

    public int KullaniciId { get; set; }
    public DateTime SiparisTarihi { get; set; } = DateTime.Now;
    public SiparisDurumu Durum { get; private set; } = SiparisDurumu.Beklemede;
    public string? Aciklama { get; set; }
    public decimal IskontoTutari { get; private set; }

    public IReadOnlyCollection<SiparisKalemi> Kalemler => _kalemler.AsReadOnly();

    public void KalemEkle(Urun urun, int miktar, decimal iskontoOrani)
    {
        if (Durum != SiparisDurumu.Beklemede)
            throw new IsKuraliException("Yalnızca beklemedeki siparişe ürün eklenebilir.");
        if (miktar <= 0)
            throw new IsKuraliException("Miktar sıfırdan büyük olmalı.");
        if (_kalemler.Any(k => k.UrunId == urun.Id))
            throw new IsKuraliException($"'{urun.Ad}' zaten siparişte. Miktarını güncelleyin.");

        _kalemler.Add(new SiparisKalemi(urun, miktar, iskontoOrani));
        HesaplaToplam();
    }

    public void KalemCikar(int urunId)
    {
        if (Durum != SiparisDurumu.Beklemede)
            throw new IsKuraliException("Yalnızca beklemedeki siparişten ürün çıkarılabilir.");
        _kalemler.RemoveAll(k => k.UrunId == urunId);
        HesaplaToplam();
    }

    public override void HesaplaToplam()
    {
        decimal brut = _kalemler.Sum(k => k.Miktar * k.BirimFiyat);
        AraToplam = _kalemler.Sum(k => k.IskontoluTutar);
        IskontoTutari = brut - AraToplam;
        KdvTutari = _kalemler.Sum(k => k.KdvTutari);
        GenelToplam = AraToplam + KdvTutari;
    }

    /// <summary>
    /// Durum geçişlerini tek yerde denetler:
    /// Beklemede -> Onaylandi -> SevkEdildi -> TeslimEdildi; Iptal yalnızca sevkten önce.
    /// </summary>
    public void DurumDegistir(SiparisDurumu yeni)
    {
        bool gecerli = (Durum, yeni) switch
        {
            (SiparisDurumu.Beklemede, SiparisDurumu.Onaylandi) => true,
            (SiparisDurumu.Onaylandi, SiparisDurumu.SevkEdildi) => true,
            (SiparisDurumu.SevkEdildi, SiparisDurumu.TeslimEdildi) => true,
            (SiparisDurumu.Beklemede, SiparisDurumu.Iptal) => true,
            (SiparisDurumu.Onaylandi, SiparisDurumu.Iptal) => true,
            _ => false
        };

        if (!gecerli)
            throw new IsKuraliException($"Sipariş '{Durum}' durumundan '{yeni}' durumuna geçemez.");

        Durum = yeni;
    }
}

/// <summary>
/// Sipariş ile ürün arasındaki çoka-çok ilişkinin ara tablosu.
/// BirimFiyat sipariş anındaki fiyatın kopyasıdır: ürüne sonradan zam gelse de değişmez.
/// </summary>
public class SiparisKalemi : BaseEntity
{
    private SiparisKalemi() { } // EF Core için

    public SiparisKalemi(Urun urun, int miktar, decimal iskontoOrani)
    {
        UrunId = urun.Id;
        Urun = urun;
        Miktar = miktar;
        BirimFiyat = urun.ListeFiyati;
        KdvOrani = urun.KdvOrani;
        IskontoOrani = iskontoOrani;
        SatirToplam = IskontoluTutar + KdvTutari;
    }

    public int SiparisId { get; set; }
    public Siparis? Siparis { get; set; }
    public int UrunId { get; private set; }
    public Urun? Urun { get; private set; }

    public int Miktar { get; private set; }
    public decimal BirimFiyat { get; private set; }
    public decimal IskontoOrani { get; private set; }
    public decimal KdvOrani { get; private set; }
    public decimal SatirToplam { get; private set; }

    public decimal IskontoluTutar => Math.Round(Miktar * BirimFiyat * (1 - IskontoOrani / 100m), 2);
    public decimal KdvTutari => Math.Round(IskontoluTutar * KdvOrani / 100m, 2);
}

public class Sevkiyat : BaseEntity
{
    public int SiparisId { get; set; }
    public Siparis? Siparis { get; set; }
    public DateTime SevkTarihi { get; set; } = DateTime.Now;
    public string? AracPlaka { get; set; }
    public string? KargoFirmasi { get; set; }
    public string? TakipNo { get; set; }
    public DateTime? TeslimTarihi { get; set; }
}

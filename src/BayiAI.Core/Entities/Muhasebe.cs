using BayiAI.Core.Enums;
using BayiAI.Core.Exceptions;

namespace BayiAI.Core.Entities;

public class Fatura : BelgeBase
{
    private Fatura() { } // EF Core için

    public string FaturaNo { get; private set; } = "";
    public override string BelgeNo => FaturaNo;

    public int SiparisId { get; private set; }
    public Siparis? Siparis { get; private set; }
    public DateTime FaturaTarihi { get; private set; }
    public DateTime VadeTarihi { get; private set; }

    public bool VadesiGectiMi => VadeTarihi.Date < DateTime.Today;

    /// <summary>Sevk edilen siparişten fatura üretir. Tutarlar o anki haliyle dondurulur.</summary>
    public static Fatura SiparistenOlustur(Siparis siparis, string faturaNo, int vadeGun)
    {
        if (!siparis.Kalemler.Any())
            throw new IsKuraliException("Kalemi olmayan siparişe fatura kesilemez.");

        var fatura = new Fatura
        {
            FaturaNo = faturaNo,
            SiparisId = siparis.Id,
            Siparis = siparis,
            BayiId = siparis.BayiId,
            FaturaTarihi = DateTime.Now,
            VadeTarihi = DateTime.Today.AddDays(vadeGun)
        };
        fatura.HesaplaToplam();
        return fatura;
    }

    public override void HesaplaToplam()
    {
        if (Siparis is null) return;
        AraToplam = Siparis.AraToplam;
        KdvTutari = Siparis.KdvTutari;
        GenelToplam = Siparis.GenelToplam;
    }
}

public class KasaBanka : BaseEntity
{
    public string Ad { get; set; } = "";
    public KasaBankaTipi Tip { get; set; }
    public string? Iban { get; set; }
}

public class Tahsilat : BaseEntity
{
    private decimal _tutar;

    public int BayiId { get; set; }
    public Bayi? Bayi { get; set; }
    public int KasaBankaId { get; set; }
    public KasaBanka? KasaBanka { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Now;
    public OdemeTipi OdemeTipi { get; set; }
    public string? Aciklama { get; set; }
    public int KullaniciId { get; set; }

    public decimal Tutar
    {
        get => _tutar;
        set
        {
            if (value <= 0) throw new IsKuraliException("Tahsilat tutarı sıfırdan büyük olmalı.");
            _tutar = value;
        }
    }
}

/// <summary>
/// Bayinin borç-alacak defterinin bir satırı.
/// Doğrudan değil, ICariIslem uygulayan sınıflar üzerinden oluşturulur.
/// </summary>
public class CariHareket : BaseEntity
{
    private CariHareket() { } // EF Core için

    public CariHareket(int bayiId, CariIslemTipi islemTipi, decimal borc, decimal alacak, string? aciklama = null)
    {
        if (borc < 0 || alacak < 0 || (borc == 0 && alacak == 0))
            throw new IsKuraliException("Cari harekette borç veya alacaktan biri pozitif olmalı.");

        BayiId = bayiId;
        IslemTipi = islemTipi;
        Borc = borc;
        Alacak = alacak;
        Aciklama = aciklama;
        Tarih = DateTime.Now;
    }

    public int BayiId { get; private set; }
    public Bayi? Bayi { get; private set; }
    public DateTime Tarih { get; private set; }
    public CariIslemTipi IslemTipi { get; private set; }
    public int? FaturaId { get; set; }
    public Fatura? Fatura { get; set; }
    public int? TahsilatId { get; set; }
    public Tahsilat? Tahsilat { get; set; }
    public decimal Borc { get; private set; }
    public decimal Alacak { get; private set; }
    public string? Aciklama { get; private set; }
}

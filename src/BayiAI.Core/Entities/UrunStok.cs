using BayiAI.Core.Enums;
using BayiAI.Core.Exceptions;

namespace BayiAI.Core.Entities;

public class Urun : BaseEntity
{
    public string UrunKodu { get; set; } = "";
    public string Ad { get; set; } = "";
    public string Birim { get; set; } = "Adet";
    public decimal ListeFiyati { get; set; }
    public decimal KdvOrani { get; set; } = 20;
    public int KritikStok { get; set; }
    public bool AktifMi { get; set; } = true;

    public int KategoriId { get; set; }
    public Kategori? Kategori { get; set; }

    /// <summary>Stok yalnızca StokGir / StokDus metotlarıyla değişir.</summary>
    public int StokMiktari { get; private set; }

    public bool KritikSeviyedeMi => StokMiktari <= KritikStok;

    public void StokGir(int miktar)
    {
        if (miktar <= 0) throw new IsKuraliException("Giriş miktarı sıfırdan büyük olmalı.");
        StokMiktari += miktar;
    }

    public void StokDus(int miktar)
    {
        if (miktar <= 0) throw new IsKuraliException("Çıkış miktarı sıfırdan büyük olmalı.");
        if (miktar > StokMiktari) throw new YetersizStokException(Ad, StokMiktari, miktar);
        StokMiktari -= miktar;
    }
}

public class StokHareket : BaseEntity
{
    public int UrunId { get; set; }
    public Urun? Urun { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Now;
    public StokHareketTipi HareketTipi { get; set; }
    public int Miktar { get; set; }
    public string? Aciklama { get; set; }
    public int? SevkiyatId { get; set; }
    public Sevkiyat? Sevkiyat { get; set; }
    public int KullaniciId { get; set; }
}

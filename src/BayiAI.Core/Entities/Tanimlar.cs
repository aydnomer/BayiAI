using BayiAI.Core.Enums;

namespace BayiAI.Core.Entities;

public class Kullanici : BaseEntity
{
    public string AdSoyad { get; set; } = "";
    public string KullaniciAdi { get; set; } = "";
    public string SifreHash { get; set; } = "";
    public KullaniciRolu Rol { get; set; }
    public bool AktifMi { get; set; } = true;
}

public class Bolge : BaseEntity
{
    public string Ad { get; set; } = "";
    public List<Bayi> Bayiler { get; set; } = new();
}

public class Kategori : BaseEntity
{
    public string Ad { get; set; } = "";

    // Kendine bağlanan ilişki: "Gazlı İçecek" -> üst kategori "İçecek"
    public int? UstKategoriId { get; set; }
    public Kategori? UstKategori { get; set; }
    public List<Kategori> AltKategoriler { get; set; } = new();

    public List<Urun> Urunler { get; set; } = new();
}

using BayiAI.Core.Enums;

namespace BayiAI.Core.Entities;

public class IslemLog : BaseEntity
{
    public int? KullaniciId { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Now;
    public string TabloAdi { get; set; } = "";
    public int KayitId { get; set; }
    public string Islem { get; set; } = "";   // Ekle / Guncelle / Sil
    public string? EskiDeger { get; set; }
    public string? YeniDeger { get; set; }
}

public class AiIslem : BaseEntity
{
    public int? KullaniciId { get; set; }
    public DateTime Tarih { get; set; } = DateTime.Now;
    public AiIslemTipi IslemTipi { get; set; }
    public string Girdi { get; set; } = "";
    public string? Cikti { get; set; }
    public int? SureMs { get; set; }
}

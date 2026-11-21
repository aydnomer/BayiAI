using BayiAI.Core.Exceptions;

namespace BayiAI.Core.Entities;

/// <summary>
/// Sistemin merkez tablosu. İskonto oranı ve risk limiti kapsülleme ile korunur:
/// geçersiz bir değer nesneye hiç giremez.
/// </summary>
public class Bayi : BaseEntity
{
    private decimal _iskontoOrani;
    private decimal _riskLimiti;

    public string BayiKodu { get; set; } = "";
    public string Unvan { get; set; } = "";
    public string VergiNo { get; set; } = "";
    public string? VergiDairesi { get; set; }
    public string? YetkiliAdSoyad { get; set; }
    public string? Telefon { get; set; }
    public string? Eposta { get; set; }
    public string? Adres { get; set; }
    public bool AktifMi { get; set; } = true;

    public int BolgeId { get; set; }
    public Bolge? Bolge { get; set; }

    public int? SorumluKullaniciId { get; set; }
    public Kullanici? SorumluKullanici { get; set; }

    public decimal IskontoOrani
    {
        get => _iskontoOrani;
        set
        {
            if (value < 0 || value > 50)
                throw new IsKuraliException("İskonto oranı 0 ile 50 arasında olmalı.");
            _iskontoOrani = value;
        }
    }

    public decimal RiskLimiti
    {
        get => _riskLimiti;
        set
        {
            if (value < 0)
                throw new IsKuraliException("Risk limiti negatif olamaz.");
            _riskLimiti = value;
        }
    }

    public List<Siparis> Siparisler { get; set; } = new();
    public List<CariHareket> CariHareketler { get; set; } = new();
}

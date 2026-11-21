using BayiAI.Core.Entities;
using BayiAI.Core.Enums;

namespace BayiAI.Core.Interfaces;

/// <summary>
/// Bayinin carisine yazılacak her işlemin ortak sözleşmesi.
/// CariServisi işlemin türünü bilmeden HareketOlustur() çağırır (çok biçimlilik).
/// </summary>
public interface ICariIslem
{
    CariHareket HareketOlustur();
}

public class FaturaIslemi : ICariIslem
{
    private readonly Fatura _fatura;
    public FaturaIslemi(Fatura fatura) => _fatura = fatura;

    public CariHareket HareketOlustur() =>
        new(_fatura.BayiId, CariIslemTipi.Fatura, borc: _fatura.GenelToplam, alacak: 0,
            aciklama: $"{_fatura.FaturaNo} nolu fatura") { Fatura = _fatura };
}

public class TahsilatIslemi : ICariIslem
{
    private readonly Tahsilat _tahsilat;
    public TahsilatIslemi(Tahsilat tahsilat) => _tahsilat = tahsilat;

    public CariHareket HareketOlustur() =>
        new(_tahsilat.BayiId, CariIslemTipi.Tahsilat, borc: 0, alacak: _tahsilat.Tutar,
            aciklama: $"{_tahsilat.OdemeTipi} tahsilat") { Tahsilat = _tahsilat };
}

public class IadeIslemi : ICariIslem
{
    private readonly int _bayiId;
    private readonly decimal _tutar;
    private readonly string _aciklama;

    public IadeIslemi(int bayiId, decimal tutar, string aciklama)
    {
        _bayiId = bayiId;
        _tutar = tutar;
        _aciklama = aciklama;
    }

    // İade bayinin borcunu azaltır: alacak tarafına yazılır.
    public CariHareket HareketOlustur() =>
        new(_bayiId, CariIslemTipi.Iade, borc: 0, alacak: _tutar, aciklama: _aciklama);
}

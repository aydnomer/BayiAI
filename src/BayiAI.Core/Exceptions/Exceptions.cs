namespace BayiAI.Core.Exceptions;

/// <summary>
/// Bir iş kuralı ihlal edildiğinde fırlatılır. Arayüz bu hatayı yakalayıp
/// mesajını kullanıcıya olduğu gibi gösterebilir.
/// </summary>
public class IsKuraliException : Exception
{
    public IsKuraliException(string mesaj) : base(mesaj) { }
}

public class RiskLimitiAsildiException : IsKuraliException
{
    public string BayiUnvan { get; }
    public decimal MevcutBakiye { get; }
    public decimal RiskLimiti { get; }

    public RiskLimitiAsildiException(string bayiUnvan, decimal mevcutBakiye, decimal riskLimiti)
        : base($"{bayiUnvan} risk limitini aşıyor. Bakiye: {mevcutBakiye:N2} TL, limit: {riskLimiti:N2} TL.")
    {
        BayiUnvan = bayiUnvan;
        MevcutBakiye = mevcutBakiye;
        RiskLimiti = riskLimiti;
    }
}

public class YetersizStokException : IsKuraliException
{
    public string UrunAd { get; }
    public int Mevcut { get; }
    public int Istenen { get; }

    public YetersizStokException(string urunAd, int mevcut, int istenen)
        : base($"'{urunAd}' için stok yetersiz. Mevcut: {mevcut}, istenen: {istenen}.")
    {
        UrunAd = urunAd;
        Mevcut = mevcut;
        Istenen = istenen;
    }
}

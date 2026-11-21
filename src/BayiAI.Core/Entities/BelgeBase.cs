namespace BayiAI.Core.Entities;

/// <summary>
/// Sipariş ve fatura gibi "belge" niteliğindeki kayıtların soyut atası.
/// Her alt sınıf toplamını kendi kuralıyla hesaplar (soyut metot + çok biçimlilik).
/// </summary>
public abstract class BelgeBase : BaseEntity
{
    public int BayiId { get; set; }
    public Bayi? Bayi { get; set; }

    public decimal AraToplam { get; protected set; }
    public decimal KdvTutari { get; protected set; }
    public decimal GenelToplam { get; protected set; }

    /// <summary>Belgenin numarası (SiparisNo, FaturaNo...).</summary>
    public abstract string BelgeNo { get; }

    /// <summary>AraToplam, KdvTutari ve GenelToplam alanlarını yeniden hesaplar.</summary>
    public abstract void HesaplaToplam();
}

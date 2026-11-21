namespace BayiAI.Core.Entities;

/// <summary>
/// Tüm tabloların ortak alanları. Her entity bu sınıftan türer (kalıtım).
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime KayitTarihi { get; protected set; } = DateTime.Now;
    public bool SilindiMi { get; private set; }

    /// <summary>Kaydı gerçekten silmez, işaretler (soft delete).</summary>
    public void Sil() => SilindiMi = true;
}
